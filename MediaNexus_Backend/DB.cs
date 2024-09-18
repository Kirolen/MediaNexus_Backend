using System;
using System.Data;
using MySql.Data.MySqlClient;
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
            // Hash the password
            string passwordHash = ComputeSha256Hash(password);

            // Check if the user login or email already exists
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
    }
}
