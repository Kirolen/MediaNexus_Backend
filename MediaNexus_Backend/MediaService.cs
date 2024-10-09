using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MediaNexus_Backend
{
    public class MediaService
    {

        static public MainMedia[] GetRecentMedia(int numMedia, int page)
        {
            return DB.GetRecentMedia(numMedia, page);
        }

        static public User CheckLogin(string loginUser, string passUser)
        {
            return DB.CheckLogin(loginUser, passUser);
        }

        static public RegisterResult Register(string userLogin, string password, string email)
        {
            return DB.Register(userLogin, password, email);
        }

        static public bool AddMediaToDatabase(Media newMedia, Genres[] genres)
        {
            return DB.AddMediaToDatabase(newMedia, genres);
        }
        public static bool AddBookToDatabase(Book newBook, Genres[] genres)
        {
            return DB.AddBookToDatabase(newBook, genres);
        }
        static public Genres[] GetGenres()
        {
            return DB.GetGenres();
        }

        public static bool ChangeUserInfo(User user, string currentPasword, string newPassword)
        {
            return DB.ChangeUserInfo(user, currentPasword, newPassword);
        }
    }
}



