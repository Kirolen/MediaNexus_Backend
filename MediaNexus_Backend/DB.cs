using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MediaNexus_Backend
{
    public enum RegisterResult
    {
        Success,
        UserLoginTaken,
        EmailTaken,
        RegistrationFailed,
        Error
    }

    internal static class DB
    {
        private static readonly string connectionString = "Server=localhost;Database=MediaNexus;User ID=root;Password=my-secret-pw;";

        public static void ConnectionOpen(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public static void ConnectionClose(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        public static DataTable GetAllMedia()
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                ConnectionOpen(connection);
                string query = "SELECT * FROM MainMedia";
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable table = new DataTable();
                adapter.Fill(table);
                ConnectionClose(connection);
                return table;
            }
        }

        public static MainMedia[] GetRecentMedia(int numMedia, int page)
        {
            int offset = (page - 1) * numMedia;

            string query = @"SELECT id, OriginalName, EnglishName, 
                             ImageURL, MediaStatus, PG_Rating, Description, IsAdded, IDUserWhoAdded, DateAdded 
                             FROM `MainMedia` 
                             ORDER BY `DateAdded` DESC, `OriginalName` DESC 
                             LIMIT @numMedia OFFSET @offset";

            List<MainMedia> mediaList = new List<MainMedia>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@numMedia", numMedia);
                        command.Parameters.AddWithValue("@offset", offset);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MainMedia media = new MainMedia
                                {
                                    Id = reader.GetInt32("id"),
                                    OriginalName = reader.GetString("OriginalName"),
                                    EnglishName = reader.IsDBNull(reader.GetOrdinal("EnglishName")) ? null : reader.GetString("EnglishName"),
                                    ImageURL = reader.GetString("ImageURL"),
                                };

                                mediaList.Add(media);
                            }
                        }
                    }
                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching recent media: {ex.Message}");
            }

            return mediaList.ToArray();
        }

        public static User CheckLogin(string loginUser, string passUser)
        {
            string hashedPassword = ComputeSha256Hash(passUser);

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                ConnectionOpen(connection);
                DataTable table = new DataTable();
                string query = "SELECT * FROM `users` WHERE `username` = @ul AND `hashPassword` = @up";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ul", loginUser);
                    command.Parameters.AddWithValue("@up", hashedPassword);
                    MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                    adapter.Fill(table);
                }

                if (table.Rows.Count > 0)
                {
                    DataRow row = table.Rows[0];

                    User user = new User
                    {
                        Id = Convert.ToInt32(row["id"]),
                        HashPassword = row["hashPassword"].ToString(),
                        UserImageURL = row.IsNull("userImageURL") ? null : row["userImageURL"].ToString(),
                        Role = (UserRole)Enum.Parse(typeof(UserRole), row["Role"].ToString()),
                        Nickname = row.IsNull("nickname") ? null : row["nickname"].ToString(),
                        RegisterDate = Convert.ToDateTime(row["registerDate"]),
                        LastLoginDate = row.IsNull("lastLoginDate") ? (DateTime?)null : Convert.ToDateTime(row["lastLoginDate"]),
                        IsBanned = Convert.ToBoolean(row["isBanned"]),
                        DateEndBan = row.IsNull("dateEndBan") ? (DateTime?)null : Convert.ToDateTime(row["dateEndBan"]),
                        Email = row["email"].ToString(),
                        BirthdayDate = row.IsNull("birthdayDate") ? (DateTime?)null : Convert.ToDateTime(row["birthdayDate"]),
                        UserDescription = row.IsNull("userDescription") ? null : row["userDescription"].ToString()
                    };

                    ConnectionClose(connection);
                    return user;
                }

                return new User();
            }
        }

        public static bool ChangeUserInfo(User user, string currentPasword, string newPassword)
        {
            user.HashPassword = CheckPassword(user.Id, currentPasword, newPassword);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);

                    string query = @"UPDATE users 
                             SET hashPassword = @newPassword, nickname = @newNickname, email = @newEmail,
                                 userDescription = @newDescription, birthdayDate = @newBirthday, userImageURL = @newImage
                             WHERE id = @userId";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@newPassword", user.HashPassword);
                        command.Parameters.AddWithValue("@newNickname", user.Nickname);
                        command.Parameters.AddWithValue("@newEmail", user.Email);
                        command.Parameters.AddWithValue("@newDescription", user.UserDescription);
                        command.Parameters.AddWithValue("@newBirthday", user.BirthdayDate.HasValue ? (object)user.BirthdayDate.Value : DBNull.Value);
                        command.Parameters.AddWithValue("@newImage", user.UserImageURL);
                        command.Parameters.AddWithValue("@userId", user.Id);

                        int result = command.ExecuteNonQuery();

                        return result > 0; 
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while updating user info: {ex.Message}");
                return false; 
            }

        }

        private static string CheckPassword(int id, string currentPassword, string newPassword)
        {
            string currentPasswordHash = ComputeSha256Hash(currentPassword);

            if (currentPasswordHash == GetPasswordHashByUserid(id))
            {
                return ComputeSha256Hash(newPassword);
            }
            else
            {
                return currentPasswordHash;
            }
        }

        public static RegisterResult Register(string userLogin, string password, string email)
        {
            string passwordHash = ComputeSha256Hash(password);

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);

                    string query = @"INSERT INTO `users` 
                             (`username`, `hashPassword`, `email`, `registerDate`, `role`, `nickname`) 
                             VALUES (@username, @passwordHash, @email, @registerDate, @role, @nickname)";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@username", userLogin);
                        command.Parameters.AddWithValue("@passwordHash", passwordHash);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@registerDate", DateTime.Now);
                        command.Parameters.AddWithValue("@role", "User");
                        command.Parameters.AddWithValue("@nickname", userLogin); // Optional: set nickname same as username initially.

                        int result = command.ExecuteNonQuery();
                        ConnectionClose(connection);
                        return result > 0 ? RegisterResult.Success : RegisterResult.RegistrationFailed;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred during registration: {ex.Message}");
                return RegisterResult.Error;
            }
        }
        public static string GetPasswordHashByUserid(int userID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);

                    string query = @"SELECT hashPassword FROM users WHERE id = @userID LIMIT 1";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userID", userID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string hashPassword = reader.GetString("hashPassword");
                                ConnectionClose(connection);
                                return hashPassword;
                            }
                        }
                    }
                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while retrieving password hash: {ex.Message}");
            }

            return null; 
        }


        private static bool IsUserLoginTaken(string userLogin)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `users` WHERE `username` = @ul", connection);
                command.Parameters.Add("@ul", MySqlDbType.VarChar).Value = userLogin;

                ConnectionOpen(connection);
                int count = Convert.ToInt32(command.ExecuteScalar());
                ConnectionClose(connection);

                return count > 0;
            }
        }
        private static bool IsEmailTaken(string email)
        {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `users` WHERE `email` = @em", connection);
                command.Parameters.Add("@em", MySqlDbType.VarChar).Value = email;

                ConnectionOpen(connection);
                int count = Convert.ToInt32(command.ExecuteScalar());
                ConnectionClose(connection);

                return count > 0;
            }
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool AddMediaToDatabase(Media newMedia, Genres[] genres)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    ConnectionOpen(connection);
                    string mainMediaQuery = @"INSERT INTO MainMedia 
                                      (MainMediaType, OriginalName, EnglishName, ImageURL, MediaStatus, PG_Rating, Description, isAdded, IDUserWhoAdded, DateAdded)
                                      VALUES 
                                      (@MainType, @OriginalName, @EnglishName, @ImageURL, @Status, @PGRating, @Description, @isAdded, @IDUserWhoAdded, @TimeAdded)";

                    using (var command = new MySqlCommand(mainMediaQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MainType", (MainMediaType)(newMedia.MainType+1));
                        command.Parameters.AddWithValue("@OriginalName", newMedia.OriginalName);
                        command.Parameters.AddWithValue("@EnglishName", newMedia.EnglishName);
                        command.Parameters.AddWithValue("@ImageURL", newMedia.ImageURL);
                        command.Parameters.AddWithValue("@Status", newMedia.Status);
                        command.Parameters.AddWithValue("@PGRating", newMedia.PG_Rating);
                        command.Parameters.AddWithValue("@Description", newMedia.Description);
                        command.Parameters.AddWithValue("@isAdded", newMedia.IsAdded);
                        command.Parameters.AddWithValue("@IDUserWhoAdded", newMedia.IDUserWhoAdded);
                        command.Parameters.AddWithValue("@TimeAdded", newMedia.TimeAdded);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            long mediaId = command.LastInsertedId;
                            string mediaQuery = @"INSERT INTO Media 
                                          (MediaId, MediaType, Studio, TotalEpisodes, ReleasedEpisode, EpisodeDuration, 
                                           TimeUntilNewEpisodeInSeconds, NextEpisodeDateTime, StartDate, EndDate)
                                          VALUES 
                                          (@MainMediaId, @SecondMediaType, @Studio, @TotalEpisodes, @ReleasedEpisode, 
                                           @EpisodeDuration, @TimeUntilNewEpisodeInSeconds, @NextEpisodeDateTime, @StartDate, @EndDate)";

                            using (var command2 = new MySqlCommand(mediaQuery, connection))
                            {
                                command2.Parameters.AddWithValue("@MainMediaId", mediaId);
                                command2.Parameters.AddWithValue("@SecondMediaType", newMedia.SecondMediaType);
                                command2.Parameters.AddWithValue("@Studio", newMedia.Studio);
                                command2.Parameters.AddWithValue("@TotalEpisodes", newMedia.TotalEpisodes);
                                command2.Parameters.AddWithValue("@ReleasedEpisode", newMedia.ReleasedEpisode);
                                command2.Parameters.AddWithValue("@EpisodeDuration", newMedia.EpisodeDuration);
                                command2.Parameters.AddWithValue("@TimeUntilNewEpisodeInSeconds", newMedia.TimeUntilNewEpisodeInSeconds);
                                command2.Parameters.AddWithValue("@NextEpisodeDateTime", newMedia.NextEpisodeDateTime.HasValue ? newMedia.NextEpisodeDateTime.Value : (object)DBNull.Value);
                                command2.Parameters.AddWithValue("@StartDate", newMedia.StartDate.HasValue ? newMedia.StartDate.Value : (object)DBNull.Value);
                                command2.Parameters.AddWithValue("@EndDate", newMedia.EndDate.HasValue ? newMedia.EndDate.Value : (object)DBNull.Value);

                                int result2 = command2.ExecuteNonQuery();
                                ConnectionClose(connection);
                                return result2 > 0 && AddConnectionBetweenMedia(mediaId, genres);
                            }
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ConnectionClose(connection);
                    return false;
                }
            }
        }


        public static bool AddBookToDatabase(Book newBook, Genres[] genres)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    ConnectionOpen(connection);
                    string mainMediaQuery = @"INSERT INTO MainMedia 
                                (MainMediaType, OriginalName, EnglishName, ImageURL, MediaStatus, PG_Rating, Description, isAdded, IDUserWhoAdded, DateAdded)
                                VALUES 
                                (@MainType, @OriginalName, @EnglishName, @ImageURL, @Status, @PGRating, @Description, @isAdded, @IDUserWhoAdded, @TimeAdded)";

                    using (var command = new MySqlCommand(mainMediaQuery, connection))
                    {
                        command.Parameters.AddWithValue("@MainType", (MainMediaType)(newBook.MainType + 1));
                        command.Parameters.AddWithValue("@OriginalName", newBook.OriginalName);
                        command.Parameters.AddWithValue("@EnglishName", newBook.EnglishName);
                        command.Parameters.AddWithValue("@ImageURL", newBook.ImageURL);
                        command.Parameters.AddWithValue("@Status", newBook.Status);
                        command.Parameters.AddWithValue("@PGRating", newBook.PG_Rating);
                        command.Parameters.AddWithValue("@Description", newBook.Description);
                        command.Parameters.AddWithValue("@isAdded", newBook.IsAdded);
                        command.Parameters.AddWithValue("@IDUserWhoAdded", newBook.IDUserWhoAdded);
                        command.Parameters.AddWithValue("@TimeAdded", newBook.TimeAdded);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            long mediaId = command.LastInsertedId;
                            string mediaQuery = @"INSERT INTO Books 
                                      (BookId, Author, PublicationDate, Pages, ISBN)
                                      VALUES 
                                      (@BookId, @Author, @PublicationDate, @Pages, @ISBN)";

                            using (var command2 = new MySqlCommand(mediaQuery, connection))
                            {
                                command2.Parameters.AddWithValue("@BookId", mediaId);
                                command2.Parameters.AddWithValue("@Author", newBook.Author);
                                command2.Parameters.AddWithValue("@PublicationDate", newBook.PublicationDate);
                                command2.Parameters.AddWithValue("@Pages", newBook.Pages);
                                command2.Parameters.AddWithValue("@ISBN", newBook.ISBN);

                                int result2 = command2.ExecuteNonQuery();
                                return result2 > 0 && AddConnectionBetweenMedia(mediaId, genres);
                            }
                        }
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return false;
                }
                finally
                {
                    ConnectionClose(connection);
                }
            }
        }


        private static bool AddConnectionBetweenMedia(long mediaID, Genres[] genres)
        {
            if (genres == null || genres.Length == 0)
            {
                Console.WriteLine("No genres provided.");
                return true;
            }

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    ConnectionOpen(connection);

                    string query = "INSERT INTO MediaGenres (MediaID, GenreID) VALUES (@MediaID, @GenreID)";

                    foreach (Genres genre in genres)
                    {
                        using (var command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@MediaID", mediaID);
                            command.Parameters.AddWithValue("@GenreID", genre.GenreID);

                            command.ExecuteNonQuery();
                        }
                    }

                    ConnectionClose(connection);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error while adding genres: {ex.Message}");
                    ConnectionClose(connection);
                    return false;
                }
            }
        }




        public static Genres[] GetGenres()
        {
            List<Genres> genresList = new List<Genres>();
            string query = "SELECT `GenreID`, `Genre` FROM `Genres`";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                MySqlCommand command = new MySqlCommand(query, connection);

                try
                {
                    connection.Open(); 
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Genres genre = new Genres(
                                reader.GetInt32("GenreID"),
                                reader.GetString("Genre"));
                            genresList.Add(genre);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching genres: " + ex.Message);
                    
                }
            } 

            return genresList.ToArray(); 
        }


    }
}
