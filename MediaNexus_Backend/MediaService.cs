namespace MediaNexus_Backend
{
    /// <summary>
    /// Provides methods to interact with media data and perform operations related to media and user responses.
    /// </summary>
    public class MediaService
    {
        /// <summary>
        /// Retrieves an array of filtered media based on the specified sort criteria, number of media items to retrieve, and page number.
        /// </summary>
        /// <param name="sortCriteria">The criteria used to filter the media.</param>
        /// <param name="numMedia">The number of media items to retrieve.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <returns>An array of <see cref="MainMedia"/> objects.</returns>
        public static MainMedia[] GetFilteredMedia(SortConditions sortCriteria, int numMedia, int page)
        {
            return DB.GetFilteredMedia(sortCriteria, numMedia, page);
        }

        /// <summary>
        /// Counts the total number of media items that match the specified sort criteria.
        /// </summary>
        /// <param name="sortCriteria">The criteria used to filter the media.</param>
        /// <returns>The count of media items that match the criteria.</returns>
        public static int CountFilteredMedia(SortConditions sortCriteria)
        {
            return DB.CountFilteredMedia(sortCriteria);
        }

        /// <summary>
        /// Retrieves the media status for a specific user and media item.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="mediaId">The unique identifier of the media item.</param>
        /// <returns>A <see cref="UserMediaStatus"/> object representing the user's media status.</returns>
        public static UserMediaStatus GetUserMediaStatus(int userId, int mediaId)
        {
            return DB.GetUserMediaStatus(userId, mediaId);
        }

        /// <summary>
        /// Adds a user response to the database.
        /// </summary>
        /// <param name="response">The <see cref="UserResponse"/> object to be added.</param>
        /// <returns><c>true</c> if the response was added successfully; otherwise, <c>false</c>.</returns>
        public static bool AddUserResponseToDatabase(UserResponse response)
        {
            return DB.AddUserResponseToDatabase(response);
        }

        /// <summary>
        /// Verifies the login credentials of a user.
        /// </summary>
        /// <param name="loginUser">The username of the user.</param>
        /// <param name="passUser">The password of the user.</param>
        /// <returns>A <see cref="User"/> object if the login is successful; otherwise, <c>null</c>.</returns>
        static public User CheckLogin(string loginUser, string passUser)
        {
            return DB.Verification(loginUser, passUser);
        }

        /// <summary>
        /// Registers a new user with the specified login, password, and email.
        /// </summary>
        /// <param name="userLogin">The login of the new user.</param>
        /// <param name="password">The password of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <returns>A <see cref="RegisterResult"/> indicating the result of the registration.</returns>
        static public RegisterResult Register(string userLogin, string password, string email)
        {
            return DB.Register(userLogin, password, email);
        }

        /// <summary>
        /// Adds a new media item to the database along with its associated genres.
        /// </summary>
        /// <param name="newMedia">The <see cref="Media"/> object to be added.</param>
        /// <param name="genres">An array of <see cref="Genres"/> representing the media genres.</param>
        /// <returns><c>true</c> if the media was added successfully; otherwise, <c>false</c>.</returns>
        static public bool AddMediaToDatabase(Media newMedia, Genres[] genres)
        {
            return DB.AddMediaToDatabase(newMedia, genres);
        }

        /// <summary>
        /// Adds a new book to the database along with its associated genres.
        /// </summary>
        /// <param name="newBook">The <see cref="Book"/> object to be added.</param>
        /// <param name="genres">An array of <see cref="Genres"/> representing the book genres.</param>
        /// <returns><c>true</c> if the book was added successfully; otherwise, <c>false</c>.</returns>
        public static bool AddBookToDatabase(Book newBook, Genres[] genres)
        {
            return DB.AddBookToDatabase(newBook, genres);
        }

        /// <summary>
        /// Retrieves an array of genres available in the database.
        /// </summary>
        /// <returns>An array of <see cref="Genres"/> objects.</returns>
        static public Genres[] GetGenres()
        {
            return DB.GetGenres();
        }

        /// <summary>
        /// Changes user information, including the password.
        /// </summary>
        /// <param name="user">The <see cref="User"/> object containing updated user information.</param>
        /// <param name="currentPasword">The current password of the user.</param>
        /// <param name="newPassword">The new password for the user.</param>
        /// <returns><c>true</c> if the user information was updated successfully; otherwise, <c>false</c>.</returns>
        public static bool ChangeUserInfo(User user, string currentPasword, string newPassword)
        {
            return DB.ChangeUserInfo(user, currentPasword, newPassword);
        }

        /// <summary>
        /// Retrieves a media item by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the media item.</param>
        /// <returns>A <see cref="Media"/> object representing the media item.</returns>
        public static Media GetMedia(int id)
        {
            return DB.GetMedia(id);
        }

        /// <summary>
        /// Adds or updates the user's media status in the database.
        /// </summary>
        /// <param name="userMediaStatus">The <see cref="UserMediaStatus"/> object representing the user's media status.</param>
        public static void AddOrUpdateUserMediaStatus(UserMediaStatus userMediaStatus)
        {
            DB.AddOrUpdateUserMediaStatus(userMediaStatus);
        }

        /// <summary>
        /// Retrieves the count of media items of a specific type.
        /// </summary>
        /// <param name="mediaType">The type of media to count.</param>
        /// <param name="mediaId">The unique identifier of the media.</param>
        /// <returns>The count of media items of the specified type.</returns>
        public static int GetMediaCountByType(MainMediaType mediaType, int mediaId)
        {
            return DB.GetMediaCountByType(mediaType, mediaId);
        }

        /// <summary>
        /// Retrieves an array of user responses for a specific media item.
        /// </summary>
        /// <param name="mediaId">The unique identifier of the media item.</param>
        /// <returns>An array of <see cref="UserResponse"/> objects.</returns>
        public static UserResponse[] GetResponsesByMediaId(int mediaId)
        {
            return DB.GetResponsesByMediaId(mediaId).ToArray();
        }
    }
}
