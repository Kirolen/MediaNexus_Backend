using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public enum MainMediaType
    {
        Book,
        Media,
        Game,
        Comics
    }

    public enum MediaStatus
    {
        Released,
        Ongoing,
        Announced,
        Canceled,
        Delayed
    }

    public enum PG_Rating
    {
        G,
        PG,
        PG_13,
        R,
        NC_17
    }


    /// <summary>
    /// Represents the main media entity in the application.
    /// </summary>
    public class MainMedia
    {
        /// <summary>
        /// Gets or sets the unique identifier for the media.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the main type of the media (e.g., Book, Media, Game, Comics).
        /// </summary>
        public MainMediaType MainType { get; set; }

        /// <summary>
        /// Gets or sets the original name of the media.
        /// </summary>
        public string OriginalName { get; set; }

        /// <summary>
        /// Gets or sets the English name of the media.
        /// </summary>
        public string EnglishName { get; set; }

        /// <summary>
        /// Gets or sets the URL of the media's image.
        /// </summary>
        public string ImageURL { get; set; }

        /// <summary>
        /// Gets or sets the current status of the media (e.g., Released, Ongoing).
        /// </summary>
        public MediaStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the parental guidance rating of the media (e.g., G, PG, R).
        /// </summary>
        public PG_Rating PG_Rating { get; set; }

        /// <summary>
        /// Gets or sets the description of the media.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the media has been added to a user's list.
        /// </summary>
        public bool? IsAdded { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the user who added the media.
        /// </summary>
        public int? IDUserWhoAdded { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the media was added.
        /// </summary>
        public DateTime? TimeAdded { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMedia"/> class.
        /// </summary>
        public MainMedia() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMedia"/> class with specified values.
        /// </summary>
        /// <param name="id">The unique identifier for the media.</param>
        /// <param name="type">The main type of the media.</param>
        /// <param name="originalName">The original name of the media.</param>
        /// <param name="englishName">The English name of the media.</param>
        /// <param name="imageUrl">The URL of the media's image.</param>
        /// <param name="status">The current status of the media.</param>
        /// <param name="pgRating">The parental guidance rating of the media.</param>
        /// <param name="description">The description of the media.</param>
        /// <param name="isAdded">Indicates whether the media has been added.</param>
        /// <param name="idUserWhoAdded">The user ID of the user who added the media.</param>
        /// <param name="timeAdded">The date and time when the media was added.</param>
        public MainMedia(int id, MainMediaType type, string originalName, string englishName,
                         string imageUrl, MediaStatus status, PG_Rating pgRating,
                         string description, bool? isAdded, int? idUserWhoAdded,
                         DateTime? timeAdded)
        {
            Id = id;
            MainType = type;
            OriginalName = originalName;
            EnglishName = englishName;
            ImageURL = imageUrl;
            Status = status;
            PG_Rating = pgRating;
            Description = description;
            IsAdded = isAdded;
            IDUserWhoAdded = idUserWhoAdded;
            TimeAdded = timeAdded;
        }

        /// <summary>
        /// Returns a string representation of the media.
        /// </summary>
        /// <returns>A string that represents the current media.</returns>
        public override string ToString()
        {
            return $"ID: {Id}, Type: {MainType}, Original Name: {OriginalName}, " +
                   $"English Name: {EnglishName}, Image URL: {ImageURL}, " +
                   $"Status: {Status}, PG Rating: {PG_Rating}, " +
                   $"Description: {Description}, Is Added: {IsAdded}, " +
                   $"User ID Who Added: {IDUserWhoAdded}, Time Added: {TimeAdded}";
        }
    }

}
