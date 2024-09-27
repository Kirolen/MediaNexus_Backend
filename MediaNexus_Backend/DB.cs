using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace MediaNexus_Backend
{
    static internal class DB
    {
        static readonly MySqlConnection conn = new MySqlConnection(MediaNexus_Backend.Properties.Settings.Default.DBLink);

        static public void OpenConection()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        static public void CloseConection()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        static public bool CheckLogin(string loginUser, string passUser)
        {
            string hashedPassword = ComputeSha256Hash(passUser);

            DataTable table = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter();
            MySqlCommand command = new MySqlCommand("SELECT * FROM `users` WHERE `Userlogin` = @ul AND `PasswordHash` = @up", conn);
            command.Parameters.Add("@ul", MySqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@up", MySqlDbType.VarChar).Value = hashedPassword;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            return table.Rows.Count > 0;
        }

        static public RegisterResult Register(string userLogin, string password, string email)
        {
            string passwordHash = ComputeSha256Hash(password);

            if (IsUserLoginTaken(userLogin))
            {
                return RegisterResult.UserLoginTaken;
            }

            if (IsEmailTaken(email))
            {
                return RegisterResult.UserLoginTaken;
            }

            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = "INSERT INTO `users` (`Userlogin`, `PasswordHash`, `Email`, `DateCreated`, `UserRole`, `Username`) VALUES (@ul, @ph, @em, @dc, @ur, @un)";
                    command.CommandType = CommandType.Text;

                    command.Parameters.Add("@ul", MySqlDbType.VarChar).Value = userLogin;
                    command.Parameters.Add("@ph", MySqlDbType.VarChar).Value = passwordHash;
                    command.Parameters.Add("@em", MySqlDbType.VarChar).Value = email;
                    command.Parameters.Add("@dc", MySqlDbType.DateTime).Value = DateTime.Now;
                    command.Parameters.Add("@ur", MySqlDbType.VarChar).Value = "User";
                    command.Parameters.Add("@un", MySqlDbType.VarChar).Value = userLogin;


                    OpenConection();
                    int result = command.ExecuteNonQuery();
                    CloseConection();


                    return result > 0 ? RegisterResult.Success : RegisterResult.RegistrationFailed;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return RegisterResult.Error;
            }
        }

        private static bool IsUserLoginTaken(string userLogin)
        {
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `users` WHERE `Userlogin` = @ul", conn);
            command.Parameters.Add("@ul", MySqlDbType.VarChar).Value = userLogin;

            OpenConection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            CloseConection();

            return count > 0;
        }

        private static bool IsEmailTaken(string email)
        {
            MySqlCommand command = new MySqlCommand("SELECT COUNT(*) FROM `users` WHERE `Email` = @em", conn);
            command.Parameters.Add("@em", MySqlDbType.VarChar).Value = email;

            OpenConection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            CloseConection();

            return count > 0;
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

        public static Media[] GetRecentMedia(int numMedia, int page)
        {
            int offset = (page - 1) * numMedia;

            string query = "SELECT * FROM `media` ORDER BY `DateAdded` DESC, `OriginalName` DESC LIMIT @numMedia OFFSET @offset";

            List<Media> mediaList = new List<Media>();

            try
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                command.Parameters.Add("@numMedia", MySqlDbType.Int32).Value = numMedia;
                command.Parameters.Add("@offset", MySqlDbType.Int32).Value = offset;

                OpenConection();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Populate a Media object with the data from the reader
                        Media media = new Media
                        {
                            MediaId = reader.GetInt32("MediaID"),
                            OriginalName = reader.GetString("OriginalName"),
                            //EnglishName = reader.GetString("EnglishName"),
                            //MediaType = (MediaTypeEnum)Enum.Parse(typeof(MediaTypeEnum), reader.GetString("MediaType")),
                            //ReleaseDate = reader.GetDateTime("ReleaseDate"),
                            Studio = reader.GetString("Studio"),
                            //Description = reader.GetString("Description"),
                            //Rating = (RatingEnum)Enum.Parse(typeof(RatingEnum), reader.GetString("Rating")),
                            //SeriesRating = reader.GetFloat("SeriesRating"),
                            //Status = (StatusEnum)Enum.Parse(typeof(StatusEnum), reader.GetString("Status")),
                            //EpisodeDuration = reader.GetTimeSpan("EpisodeDuration"),
                            //TotalEpisodes = reader.GetInt32("TotalEpisodes"),
                            //ReleasedEpisodes = reader.GetInt32("ReleasedEpisodes"),
                            //NextEpisode = reader.IsDBNull(reader.GetOrdinal("NextEpisode")) ? (DateTime?)null : reader.GetDateTime("NextEpisode"),
                            //TimeUntilNextEpisode = reader.IsDBNull(reader.GetOrdinal("TimeUntilNextEpisode")) ? (DateTime?)null : reader.GetDateTime("TimeUntilNextEpisode"),
                            //IsAdded = reader.GetBoolean("isAdded"),
                            ImageTitle = reader.GetString("ImageURL")
                        };

                        mediaList.Add(media);
                    }
                }

                CloseConection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return mediaList.ToArray();
        }



        static public int GetMediaCount(string table)
        {
            int mediaCount = 0;
            string query = $"SELECT COUNT(*) FROM `{table}`";

            try
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                OpenConection();
                mediaCount = Convert.ToInt32(command.ExecuteScalar());
                CloseConection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return mediaCount;
        }

        static public Genres[] GetGenres()
        {
            List<Genres> genresList = new List<Genres>();
            string query = "SELECT `GenreID`, `GenreName` FROM `genres`";

            try
            {
                MySqlCommand command = new MySqlCommand(query, conn);
                OpenConection();

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        Genres genre = new Genres(
                     reader.GetInt32("GenreID"),
                     reader.GetString("GenreName")
                 );
                        genresList.Add(genre);
                    }
                }

                CloseConection();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                CloseConection(); // Ensure the connection is closed on error
            }

            return genresList.ToArray(); // Return the list as an array
        }


        public static void addMediaToDatabase(Media media, Genres[] genres)
        {
            try
            {
                using (MySqlCommand command = new MySqlCommand())
                {
                    command.Connection = conn;
                    command.CommandText = @"
            INSERT INTO media 
            (OriginalName, EnglishName, ImageURL, MediaType, ReleaseDate, Studio, Description, Rating, Status, EpisodeDuration, TotalEpisodes, ReleasedEpisodes, NextEpisode, TimeUntilNextEpisode)
            VALUES 
            (@OriginalName, @EnglishName, @ImageURL, @MediaType, @ReleaseDate, @Studio, @Description, @Rating, @Status, @EpisodeDuration, @TotalEpisodes, @ReleasedEpisodes, @NextEpisode, @TimeUntilNextEpisode)"; // Fixed parameter name

                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@OriginalName", media.OriginalName);
                    command.Parameters.AddWithValue("@EnglishName", media.EnglishName);
                    command.Parameters.AddWithValue("@ImageURL", media.ImageTitle);
                    command.Parameters.AddWithValue("@MediaType", media.MediaType);
                    command.Parameters.AddWithValue("@ReleaseDate", media.ReleaseDate.Date);
                    command.Parameters.AddWithValue("@Studio", media.Studio);
                    command.Parameters.AddWithValue("@Description", media.Description);
                    command.Parameters.AddWithValue("@Rating", media.Rating);
                    command.Parameters.AddWithValue("@Status", media.Status);
                    command.Parameters.AddWithValue("@EpisodeDuration", media.EpisodeDuration);
                    command.Parameters.AddWithValue("@TotalEpisodes", media.TotalEpisodes);
                    command.Parameters.AddWithValue("@ReleasedEpisodes", media.ReleasedEpisodes);
                    command.Parameters.AddWithValue("@NextEpisode", media.NextEpisode);
                    command.Parameters.AddWithValue("@TimeUntilNextEpisode", media.TimeUntilNextEpisode);

                    OpenConection();
                    int result = command.ExecuteNonQuery(); 
                    CloseConection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); 
            }
        }



    }
}
