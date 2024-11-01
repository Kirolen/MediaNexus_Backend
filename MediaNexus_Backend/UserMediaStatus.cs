namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents the possible statuses of media in a user's list.
    /// </summary>
    public enum MediaStatusInUserList
    {
        /// <summary>
        /// Indicates that the media is currently being processed.
        /// </summary>
        InProcess,
        /// <summary>
        /// Indicates that the media has been completed.
        /// </summary>
        Completed,
        /// <summary>
        /// Indicates that the media is planned to be watched or read.
        /// </summary>
        Planned,
        /// <summary>
        /// Indicates that the media has been dropped.
        /// </summary>
        Dropped
    }

    /// <summary>
    /// Represents the media status for a specific user, including the media ID, user ID, 
    /// current status, and the last ended page or episode.
    /// </summary>
    public class UserMediaStatus
    {
        /// <summary>
        /// Gets or sets the unique identifier for the media.
        /// </summary>
        public int MediaID { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// Gets or sets the current status of the media for the user.
        /// </summary>
        public MediaStatusInUserList Status { get; set; }

        /// <summary>
        /// Gets or sets the last ended page or episode of the media.
        /// </summary>
        public int EndedPageOrEpisode { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserMediaStatus"/> class.
        /// </summary>
        /// <param name="mediaId">The unique identifier for the media.</param>
        /// <param name="userId">The unique identifier for the user.</param>
        /// <param name="status">The current status of the media for the user.</param>
        /// <param name="endedPageOrEpisode">The last ended page or episode of the media (default is 0).</param>
        public UserMediaStatus(int mediaId, int userId, MediaStatusInUserList status, int endedPageOrEpisode = 0)
        {
            MediaID = mediaId;
            UserID = userId;
            Status = status;
            EndedPageOrEpisode = endedPageOrEpisode;
        }

        /// <summary>
        /// Returns a string representation of the <see cref="UserMediaStatus"/> instance.
        /// </summary>
        /// <returns>A string that represents the current <see cref="UserMediaStatus"/>.</returns>
        public override string ToString()
        {
            return $"MediaID: {MediaID}, UserID: {UserID}, Status: {Status}, EndedPageOrEpisode: {EndedPageOrEpisode}";
        }
    }
}
