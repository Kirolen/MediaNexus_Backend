﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
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
    /// <summary>
    /// Provides methods for database operations related to media management.
    /// </summary>
    internal static class DB
    {
        /// <summary>
        /// The connection string for the database.
        /// </summary>
        private static readonly string connectionString = "Server=localhost;Database=MediaNexus;User ID=root;Password=my-secret-pw;";

        /// <summary>
        /// Opens the specified database connection if it is closed.
        /// </summary>
        /// <param name="connection">The database connection to open.</param>
        public static void ConnectionOpen(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        /// <summary>
        /// Closes the specified database connection if it is open.
        /// </summary>
        /// <param name="connection">The database connection to close.</param>
        public static void ConnectionClose(MySqlConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Counts the number of filtered media based on the specified sorting criteria.
        /// </summary>
        /// <param name="sortCriteria">The criteria used to filter media.</param>
        /// <returns>The total count of filtered media.</returns>
        public static int CountFilteredMedia(SortConditions sortCriteria)
        {
            var queryBuilder = new StringBuilder("SELECT COUNT(DISTINCT mm.id) FROM MainMedia mm ");
            queryBuilder.Append("LEFT JOIN MediaGenres mg ON mm.id = mg.MediaID ");
            queryBuilder.Append("LEFT JOIN UserMediaStatus ums ON mm.id = ums.MediaID AND ums.UserID = @UserID ");

            var conditions = new List<string>();

            if (sortCriteria.selectedTypes.Length > 0)
                conditions.Add("mm.MainMediaType IN (" + string.Join(", ", sortCriteria.selectedTypes.Select(t => $"'{t}'")) + ")");

            if (sortCriteria.selectedGenres.Length > 0)
                conditions.Add("mg.GenreID IN (" + string.Join(", ", sortCriteria.selectedGenres.Select(g => g.GenreID)) + ")");

            if (sortCriteria.selectedMediaStatus.Length > 0)
                conditions.Add("mm.MediaStatus IN (" + string.Join(", ", sortCriteria.selectedMediaStatus.Select(s => $"'{s}'")) + ")");

            if (sortCriteria.selectedStatus.Length > 0)
                conditions.Add("ums.Status IN (" + string.Join(", ", sortCriteria.selectedStatus.Select(s => $"'{s}'")) + ")");

            if (conditions.Count > 0)
                queryBuilder.Append("WHERE ");
                queryBuilder.Append(string.Join(" AND ", conditions));

            string query = queryBuilder.ToString();
            Console.WriteLine(query);
            int total = 0;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserID", sortCriteria.userID);
                        total = Convert.ToInt32(command.ExecuteScalar());
                    }
                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while counting filtered media: {ex.Message}");
            }

            return total;
        }

        /// <summary>
        /// Retrieves filtered media based on the specified sorting criteria and pagination settings.
        /// </summary>
        /// <param name="sortCriteria">The criteria used to filter media.</param>
        /// <param name="numMedia">The number of media items to retrieve.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <returns>An array of filtered media items.</returns>
        public static MainMedia[] GetFilteredMedia(SortConditions sortCriteria, int numMedia, int page)
        {
            int offset = (page - 1) * numMedia;

            var queryBuilder = new StringBuilder("SELECT DISTINCT mm.* FROM MainMedia mm ");
            queryBuilder.Append("LEFT JOIN MediaGenres mg ON mm.id = mg.MediaID ");
            queryBuilder.Append("LEFT JOIN UserMediaStatus ums ON mm.id = ums.MediaID AND ums.UserID = @UserID ");

            var conditions = new List<string>();

            if (sortCriteria.selectedTypes.Length > 0)
                conditions.Add("mm.MainMediaType IN (" + string.Join(", ", sortCriteria.selectedTypes.Select(t => $"'{t}'")) + ")");

            if (sortCriteria.selectedGenres.Length > 0)
                conditions.Add("mg.GenreID IN (" + string.Join(", ", sortCriteria.selectedGenres.Select(g => g.GenreID)) + ")");
            

            if (sortCriteria.selectedMediaStatus.Length > 0)
                conditions.Add("mm.MediaStatus IN (" + string.Join(", ", sortCriteria.selectedMediaStatus.Select(s => $"'{s}'")) + ")");

            if (sortCriteria.selectedStatus.Length > 0)
                conditions.Add("ums.Status IN (" + string.Join(", ", sortCriteria.selectedStatus.Select(s => $"'{s}'")) + ")");

            if (conditions.Count > 0)
            {
                queryBuilder.Append("WHERE ");
                queryBuilder.Append(string.Join(" AND ", conditions));
            }

            queryBuilder.Append(" ORDER BY DateAdded DESC, OriginalName DESC ");
            queryBuilder.Append("LIMIT @total OFFSET @offset");

            string query = queryBuilder.ToString();
            Console.WriteLine(query);
            List<MainMedia> mediaList = new List<MainMedia>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@total", numMedia);
                        command.Parameters.AddWithValue("@offset", offset);
                        command.Parameters.AddWithValue("@UserID", sortCriteria.userID);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MainMedia media = new MainMedia
                                {
                                    Id = reader.GetInt32("id"),
                                    MainType = (MainMediaType)Enum.Parse(typeof(MainMediaType), reader.GetString("MainMediaType")),
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
                Console.WriteLine($"An error occurred while fetching filtered media: {ex.Message}");
            }

            return mediaList.ToArray();
        }

        /// <summary>
        /// Retrieves a specific media item by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the media to retrieve.</param>
        /// <returns>The media item if found; otherwise, null.</returns>
        public static Media GetMedia(int id)
        {
            string query = @"SELECT * 
                    FROM MainMedia 
                    JOIN Media ON MainMedia.id = Media.MediaId 
                    WHERE MainMedia.id = @id;";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read()) 
                            {
                                Media media = new Media
                                {
                                    Id = reader.GetInt32("id"),
                                    MainType = (MainMediaType)Enum.Parse(typeof(MainMediaType), reader.GetString("MainMediaType")),
                                    OriginalName = reader.GetString("OriginalName"),
                                    EnglishName = reader.IsDBNull(reader.GetOrdinal("EnglishName")) ? null : reader.GetString("EnglishName"),
                                    ImageURL = reader.GetString("ImageURL"),
                                    Status = (MediaStatus)Enum.Parse(typeof(MediaStatus), reader.GetString("MediaStatus")),
                                    PG_Rating = (PG_Rating)Enum.Parse(typeof(PG_Rating), reader.GetString("PG_Rating")),
                                    Description = reader.GetString("Description"),
                                    IsAdded = reader.GetBoolean("isAdded"),
                                    IDUserWhoAdded = reader.GetInt32("IDUserWhoAdded"),
                                    TimeAdded = reader.GetDateTime("DateAdded"),
                                    SecondMediaType = (MediaType)Enum.Parse(typeof(MediaType), reader.GetString("MediaType")),
                                    Studio = reader.GetString("Studio"),
                                    TotalEpisodes = reader.GetInt32("TotalEpisodes"),
                                    ReleasedEpisode = reader.GetInt32("ReleasedEpisode"),
                                    EpisodeDuration = reader.GetInt32("EpisodeDuration"),
                                    TimeUntilNewEpisodeInSeconds = reader.GetInt32("TimeUntilNewEpisodeInSeconds"),
                                    StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? (DateTime?)null : reader.GetDateTime("StartDate"),
                                    EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? (DateTime?)null : reader.GetDateTime("EndDate"),
                                };

                                return media; 
                            }
                        }
                    }

                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching media: {ex.Message}");
            }

            return null;
        }

        /// <summary>
        /// Verifies the user's credentials by checking the provided username and password against the database.
        /// </summary>
        /// <param name="loginUser">The username of the user attempting to log in.</param>
        /// <param name="passUser">The password of the user attempting to log in.</param>
        /// <returns>
        /// A <see cref="User"/> object if the credentials are valid; otherwise, a new <see cref="User"/> object with default values.
        /// </returns>
        public static User Verification(string loginUser, string passUser)
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
                        LastLoginDate = DateTime.Now,
                        IsBanned = Convert.ToBoolean(row["isBanned"]),
                        DateEndBan = row.IsNull("dateEndBan") ? (DateTime?)null : Convert.ToDateTime(row["dateEndBan"]),
                        Email = row["email"].ToString(),
                        BirthdayDate = row.IsNull("birthdayDate") ? (DateTime?)null : Convert.ToDateTime(row["birthdayDate"]),
                        UserDescription = row.IsNull("userDescription") ? null : row["userDescription"].ToString()
                    };

                    string updateQuery = "UPDATE `users` SET `lastLoginDate` = @lastLoginDate WHERE `id` = @userId";
                    using (MySqlCommand updateCommand = new MySqlCommand(updateQuery, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@lastLoginDate", DateTime.Now);
                        updateCommand.Parameters.AddWithValue("@userId", user.Id);
                        updateCommand.ExecuteNonQuery();
                    }

                    ConnectionClose(connection);
                    return user;
                }

                return new User();
            }
        }

        /// <summary>
        /// Changes the user's information, including updating the password if the current password is verified.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object containing the updated user information.</param>
        /// <param name="currentPasword">The user's current password for verification.</param>
        /// <param name="newPassword">The new password to set for the user.</param>
        /// <returns>
        /// True if the user information was successfully updated; otherwise, false.
        /// </returns>
        public static bool ChangeUserInfo(User user, string currentPasword, string newPassword)
        {
            user.HashPassword = CheckPassword(user.Id, currentPasword, newPassword);
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    string email = IsEmailTaken(user.Email) ? " " : ", email = @newEmail ";
                    string query = @"UPDATE users 
                             SET userImageURL = @newImage, hashPassword = @newPassword, nickname = @newNickname,
                             userDescription = @newDescription, birthdayDate = @newBirthday" + email +
                             "WHERE id = @userId";

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

        /// <summary>
        /// Retrieves the password hash of a user based on their unique identifier.
        /// </summary>
        /// <param name="userID">The unique identifier of the user.</param>
        /// <returns>
        /// The hashed password of the user if found; otherwise, null.
        /// </returns>
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

        /// <summary>
        /// Checks the provided password against the stored hash to determine if the current password is correct, and updates the password if valid.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <param name="currentPassword">The user's current password.</param>
        /// <param name="newPassword">The new password to be set.</param>
        /// <returns>
        /// The new password hash if the current password is valid; otherwise, the existing password hash.
        /// </returns>
        private static string CheckPassword(int id, string currentPassword, string newPassword)
        {
            string currentPasswordHash = ComputeSha256Hash(currentPassword);
            string password = GetPasswordHashByUserid(id);

            if (currentPasswordHash == password)
            {
                return ComputeSha256Hash(newPassword);
            }
            else
            {
                return password;
            }
        }

        /// <summary>
        /// Registers a new user in the system by storing their login, password, and email in the database.
        /// </summary>
        /// <param name="userLogin">The username of the user registering.</param>
        /// <param name="password">The password for the user account.</param>
        /// <param name="email">The email address of the user.</param>
        /// <returns>
        /// A <see cref="RegisterResult"/> indicating the outcome of the registration process.
        /// </returns>
        public static RegisterResult Register(string userLogin, string password, string email)
        {
            if (IsEmailTaken(email)) return RegisterResult.EmailTaken;
            if (IsUserLoginTaken(userLogin)) return RegisterResult.UserLoginTaken;

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
                        command.Parameters.AddWithValue("@nickname", userLogin); 

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

        /// <summary>
        /// Adds a user response to the database for a specific media item.
        /// </summary>
        /// <param name="response">The <see cref="UserResponse"/> object containing the response details.</param>
        /// <returns>
        /// True if the response was successfully added; otherwise, false.
        /// </returns>
        public static bool AddUserResponseToDatabase(UserResponse response)
        {
            string query = @"INSERT INTO UserResponses (userID, mediaID, ResponseText, ResponseType)
                     VALUES (@userID, @mediaID, @responseText, @responseType)";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userID", response.UserID);
                        command.Parameters.AddWithValue("@mediaID", response.MediaID);
                        command.Parameters.AddWithValue("@responseText", response.ResponseText);
                        command.Parameters.AddWithValue("@responseType", response.ResponseType.ToString());

                        int result = command.ExecuteNonQuery();
                        ConnectionClose(connection);
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding response to database: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Retrieves a list of user responses for a specific media item from the database.
        /// </summary>
        /// <param name="mediaId">The unique identifier of the media item.</param>
        /// <returns>
        /// A list of <see cref="UserResponse"/> objects associated with the specified media item.
        /// </returns>
        public static List<UserResponse> GetResponsesByMediaId(int mediaId)
        {
            List<UserResponse> responses = new List<UserResponse>();

            string query = @"
    SELECT ur.UserID, ur.ResponseID, ur.MediaID, ur.ResponseText, ur.ResponseType, 
           u.nickname AS UserNickname, u.userImageURL AS UserImgUrl
    FROM UserResponses ur
    JOIN users u ON ur.UserID = u.id
    WHERE ur.MediaID = @mediaId";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@mediaId", mediaId);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            UserResponse response = new UserResponse(
                                userID: reader.GetInt32("UserID"),
                                responseID: reader.GetInt32("ResponseID"),
                                mediaID: reader.GetInt32("MediaID"),
                                responseText: reader.GetString("ResponseText"),
                                responseType: (ResponseType)Enum.Parse(typeof(ResponseType), reader.GetString("ResponseType")),
                                userNickname: reader.GetString("UserNickname"),
                                userIMGURL: reader.IsDBNull(reader.GetOrdinal("UserImgUrl")) ? null : reader.GetString("UserImgUrl")
                            );

                            responses.Add(response);
                        }
                    }
                }
            }
            return responses;
        }

        /// <summary>
        /// Checks if the specified username is already taken in the database.
        /// </summary>
        /// <param name="userLogin">The username to check for availability.</param>
        /// <returns>
        /// True if the username is taken; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Checks if the specified email is already associated with an account in the database.
        /// </summary>
        /// <param name="email">The email address to check for availability.</param>
        /// <returns>
        /// True if the email is already taken; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Computes the SHA256 hash of the provided raw data.
        /// </summary>
        /// <param name="rawData">The raw data to be hashed.</param>
        /// <returns>
        /// A hexadecimal string representation of the SHA256 hash.
        /// </returns>
        [DebuggerStepThrough]
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

        /// <summary>
        /// Adds a new media entry to the database, including its associated genres.
        /// </summary>
        /// <param name="newMedia">The media object containing the information to be added.</param>
        /// <param name="genres">An array of genres associated with the media.</param>
        /// <returns>
        /// True if the media was added successfully; otherwise, false.
        /// </returns>
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
                        command.Parameters.AddWithValue("@MainType", (MainMediaType)(newMedia.MainType + 1));
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

        /// <summary>
        /// Adds a new book entry to the database, including its associated genres.
        /// </summary>
        /// <param name="newBook">The book object containing the information to be added.</param>
        /// <param name="genres">An array of genres associated with the book.</param>
        /// <returns>
        /// True if the book was added successfully; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Adds connections between media items and their associated genres to the database.
        /// </summary>
        /// <param name="mediaID">The ID of the media item to associate with genres.</param>
        /// <param name="genres">An array of genres to associate with the media item.</param>
        /// <returns>
        /// True if the associations were added successfully; otherwise, false.
        /// </returns>
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

        /// <summary>
        /// Retrieves all genres from the database.
        /// </summary>
        /// <returns>
        /// An array of <see cref="Genres"/> objects containing the available genres.
        /// </returns>
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


        /// <summary>
        /// Retrieves the media status for a specific user and media item.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="mediaId">The ID of the media item.</param>
        /// <returns>
        /// An instance of <see cref="UserMediaStatus"/> containing the user's media status, 
        /// or null if no status exists for the user and media item.
        /// </returns>
        public static UserMediaStatus GetUserMediaStatus(int userId, int mediaId)
        {
            string query = @"SELECT MediaID, UserID, Status, EndedPageOrEpisode
                     FROM UserMediaStatus
                     WHERE UserID = @userId AND MediaID = @mediaId";

            UserMediaStatus userMediaStatus = null;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@userId", userId);
                        command.Parameters.AddWithValue("@mediaId", mediaId);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userMediaStatus = new UserMediaStatus(
                                    mediaId,
                                    userId,
                                    (MediaStatusInUserList)Enum.Parse(typeof(MediaStatusInUserList), reader.GetString("Status")),
                                    reader.GetInt32("EndedPageOrEpisode")
                                );
                            }
                        }
                    }
                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while fetching UserMediaStatus: {ex.Message}");
            }

            return userMediaStatus;
        }


        /// <summary>
        /// Adds a new user media status or updates an existing one in the database.
        /// </summary>
        /// <param name="userMediaStatus">The <see cref="UserMediaStatus"/> object containing the status to be added or updated.</param>
        public static void AddOrUpdateUserMediaStatus(UserMediaStatus userMediaStatus)
        {
            string queryCheck = @"SELECT COUNT(*) FROM UserMediaStatus WHERE UserID = @userId AND MediaID = @mediaId";
            string queryInsert = @"INSERT INTO UserMediaStatus (UserID, MediaID, Status, EndedPageOrEpisode) 
                           VALUES (@userId, @mediaId, @status, @endedPageOrEpisode)";
            string queryUpdate = @"UPDATE UserMediaStatus 
                           SET Status = @status, EndedPageOrEpisode = @endedPageOrEpisode 
                           WHERE UserID = @userId AND MediaID = @mediaId";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    ConnectionOpen(connection);

                    // Перевірка наявності запису
                    using (MySqlCommand commandCheck = new MySqlCommand(queryCheck, connection))
                    {
                        commandCheck.Parameters.AddWithValue("@userId", userMediaStatus.UserID);
                        commandCheck.Parameters.AddWithValue("@mediaId", userMediaStatus.MediaID);

                        int count = Convert.ToInt32(commandCheck.ExecuteScalar());

                        if (count == 0)
                        {
                            // Додавання нового запису
                            using (MySqlCommand commandInsert = new MySqlCommand(queryInsert, connection))
                            {
                                commandInsert.Parameters.AddWithValue("@userId", userMediaStatus.UserID);
                                commandInsert.Parameters.AddWithValue("@mediaId", userMediaStatus.MediaID);
                                commandInsert.Parameters.AddWithValue("@status", userMediaStatus.Status.ToString());
                                commandInsert.Parameters.AddWithValue("@endedPageOrEpisode", userMediaStatus.EndedPageOrEpisode);

                                commandInsert.ExecuteNonQuery();
                            }
                        }
                        else
                        {
                            // Оновлення існуючого запису
                            using (MySqlCommand commandUpdate = new MySqlCommand(queryUpdate, connection))
                            {
                                commandUpdate.Parameters.AddWithValue("@userId", userMediaStatus.UserID);
                                commandUpdate.Parameters.AddWithValue("@mediaId", userMediaStatus.MediaID);
                                commandUpdate.Parameters.AddWithValue("@status", userMediaStatus.Status.ToString());
                                commandUpdate.Parameters.AddWithValue("@endedPageOrEpisode", userMediaStatus.EndedPageOrEpisode);


                                commandUpdate.ExecuteNonQuery();
                            }
                        }
                    }

                    ConnectionClose(connection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while adding or updating UserMediaStatus: {ex.Message}");
            }
        }


        /// <summary>
        /// Retrieves the count of pages or episodes for a specific media type.
        /// </summary>
        /// <param name="mediaType">The type of media (e.g., Book or Media).</param>
        /// <param name="mediaId">The ID of the media item.</param>
        /// <returns>
        /// The count of pages or episodes for the specified media type, or 0 if not found or if an error occurs.
        /// </returns>
        public static int GetMediaCountByType(MainMediaType mediaType, int mediaId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    ConnectionOpen(connection);
                    string query = string.Empty;

                    if (mediaType == MainMediaType.Book)
                    {
                        query = "SELECT Pages FROM Books WHERE BookId = @MediaId";
                    }
                    else if (mediaType == MainMediaType.Media)
                    {
                        query = "SELECT TotalEpisodes FROM Media WHERE MediaId = @MediaId";
                    }
                    else
                    {
                        throw new ArgumentException("Invalid media type");
                    }

                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@MediaId", mediaId);

                        var result = command.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0; // Повертає кількість або 0, якщо не знайдено
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}\nStack Trace: {ex.StackTrace}");
                    return 0;
                }
                finally
                {
                    ConnectionClose(connection);
                }
            }
        }

    }
}
