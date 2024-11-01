

namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents a response made by a user to a media item.
    /// </summary>
    public enum ResponseType
    {
        /// <summary>
        /// Indicates a neutral response.
        /// </summary>
        Neutral,
        /// <summary>
        /// Indicates a negative response.
        /// </summary>
        Negative,
        /// <summary>
        /// Indicates a positive response.
        /// </summary>
        Positive
    }

    /// <summary>
    /// Represents a user's response to a media item, including details about the user and their feedback.
    /// </summary>
    public class UserResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the user who made the response.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the response.
        /// </summary>
        public int ResponseID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the media item the response is associated with.
        /// </summary>
        public int MediaID { get; set; }

        /// <summary>
        /// Gets or sets the text of the user's response.
        /// </summary>
        public string ResponseText { get; set; }

        /// <summary>
        /// Gets or sets the type of response (e.g., Neutral, Negative, Positive).
        /// </summary>
        public ResponseType ResponseType { get; set; } = ResponseType.Neutral;

        /// <summary>
        /// Gets or sets the nickname of the user who made the response.
        /// </summary>
        public string UserNickname { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile image.
        /// </summary>
        public string UserIMGURL { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserResponse"/> class with specified values.
        /// </summary>
        /// <param name="userID">The unique identifier of the user.</param>
        /// <param name="responseID">The unique identifier of the response.</param>
        /// <param name="mediaID">The unique identifier of the media item.</param>
        /// <param name="responseText">The text of the user's response.</param>
        /// <param name="responseType">The type of response.</param>
        /// <param name="userNickname">The nickname of the user.</param>
        /// <param name="userIMGURL">The URL of the user's profile image.</param>
        public UserResponse(int userID, int responseID, int mediaID, string responseText, ResponseType responseType, string userNickname, string userIMGURL)
        {
            UserID = userID;
            ResponseID = responseID;
            MediaID = mediaID;
            ResponseText = responseText;
            ResponseType = responseType;
            UserNickname = userNickname;
            UserIMGURL = userIMGURL;
        }
    }
}
