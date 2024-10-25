using System;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MediaNexus_Backend
{
    public class MediaService
    {
        public static MainMedia[] GetFilteredMedia(SortMediacs sortCriteria, int numMedia, int page)
        {
            return DB.GetFilteredMedia(sortCriteria, numMedia, page);
        }

        public static int CountFilteredMedia(SortMediacs sortCriteria)
        {
            return DB.CountFilteredMedia(sortCriteria);
        }

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

        public static Media GetMedia(int id)
        {
            return DB.GetMedia(id);
        }

        public static int GetMediaCount() {
            return DB.GetMainMediaCount();
        }

        public static void GetSelectedMedia(string[] selectedTypes, Genres[] selectedGenres, string[] selectedStatus)
        {
            DB.GetSelectedMedia(selectedTypes, selectedGenres, selectedStatus);
        }
    }
}



