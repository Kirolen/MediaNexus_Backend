using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace MediaNexus_Backend
{
    public class MediaService
    {
        public static MainMedia[] GetFilteredMedia(SortMedia sortCriteria, int numMedia, int page)
        {
            return DB.GetFilteredMedia(sortCriteria, numMedia, page);
        }

        public static int CountFilteredMedia(SortMedia sortCriteria)
        {
            return DB.CountFilteredMedia(sortCriteria);
        }

        public static UserMediaStatus GetUserMediaStatus(int userId, int mediaId)
        {
            return DB.GetUserMediaStatus(userId, mediaId);
        }

        public static bool AddUserResponseToDatabase(UserResponse response)
        {
            return DB.AddUserResponseToDatabase(response);
        }
        static public User CheckLogin(string loginUser, string passUser)
        {
            return DB.Verification(loginUser, passUser);
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

        public static void AddOrUpdateUserMediaStatus(UserMediaStatus userMediaStatus)
        {
            DB.AddOrUpdateUserMediaStatus(userMediaStatus);
        }

        public static int GetMediaCountByType(MainMediaType mediaType, int mediaId)
        {
            return DB.GetMediaCountByType(mediaType, mediaId);
        }

        public static UserResponse[] GetResponsesByMediaId(int mediaId)
        {
            return DB.GetResponsesByMediaId(mediaId).ToArray();
        }
    }
}



