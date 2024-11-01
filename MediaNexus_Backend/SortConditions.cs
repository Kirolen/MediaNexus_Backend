using System;
namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents the conditions used for sorting and filtering media items in a user's list.
    /// </summary>
    public class SortConditions
    {
        /// <summary>
        /// Gets or sets the selected media types for filtering.
        /// </summary>
        public string[] selectedTypes = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the selected genres for filtering.
        /// </summary>
        public Genres[] selectedGenres = Array.Empty<Genres>();

        /// <summary>
        /// Gets or sets the selected statuses for filtering media items.
        /// </summary>
        public string[] selectedStatus = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the selected media statuses for filtering.
        /// </summary>
        public string[] selectedMediaStatus = Array.Empty<string>();

        /// <summary>
        /// Gets or sets the total number of media items that match the filtering criteria.
        /// </summary>
        public int Total;

        /// <summary>
        /// Gets or sets the unique identifier of the user associated with these sorting conditions.
        /// </summary>
        public int userID;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortConditions"/> class with specified sorting conditions.
        /// </summary>
        /// <param name="Types">The array of selected media types.</param>
        /// <param name="genres">The array of selected genres.</param>
        /// <param name="status">The array of selected statuses.</param>
        /// <param name="mediaStatus">The array of selected media statuses.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        public SortConditions(string[] Types, Genres[] genres, string[] status, string[] mediaStatus, int userId)
        {
            selectedTypes = Types;
            selectedGenres = genres;
            selectedStatus = status;
            selectedMediaStatus = mediaStatus;
            userID = userId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortConditions"/> class with default values.
        /// </summary>
        public SortConditions() { }
    }
}
