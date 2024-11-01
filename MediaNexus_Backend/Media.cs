using System;

namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents a media item, which can be either a movie or a serial, 
    /// and contains detailed information about the media's production and release status.
    /// </summary>
    public enum MediaType
    {
        /// <summary>
        /// Indicates that the media type is a movie.
        /// </summary>
        Movie,

        /// <summary>
        /// Indicates that the media type is a serial.
        /// </summary>
        Serial
    }

    /// <summary>
    /// Represents a media item, inheriting from <see cref="MainMedia"/>.
    /// </summary>
    public class Media : MainMedia
    {
        /// <summary>
        /// Gets or sets the secondary media type (Movie or Serial).
        /// </summary>
        public MediaType SecondMediaType { get; set; }

        /// <summary>
        /// Gets or sets the studio that produced the media.
        /// </summary>
        public string Studio { get; set; }

        /// <summary>
        /// Gets or sets the total number of episodes for the serial. 
        /// Null if not applicable.
        /// </summary>
        public int? TotalEpisodes { get; set; }

        /// <summary>
        /// Gets or sets the number of released episodes.
        /// </summary>
        public int ReleasedEpisode { get; set; }

        /// <summary>
        /// Gets or sets the duration of an episode in minutes. 
        /// Null if not applicable.
        /// </summary>
        public int? EpisodeDuration { get; set; }

        /// <summary>
        /// Gets or sets the time until the next episode is released, in seconds. 
        /// Null if not applicable.
        /// </summary>
        public int? TimeUntilNewEpisodeInSeconds { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the next episode's release.
        /// Null if not applicable.
        /// </summary>
        public DateTime? NextEpisodeDateTime { get; set; }

        /// <summary>
        /// Gets or sets the start date of the media.
        /// Null if not applicable.
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the end date of the media.
        /// Null if not applicable.
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Media"/> class.
        /// </summary>
        public Media() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Media"/> class 
        /// with the specified parameters.
        /// </summary>
        /// <param name="mediaId">The unique identifier of the media.</param>
        /// <param name="type">The main type of the media.</param>
        /// <param name="originalName">The original name of the media.</param>
        /// <param name="englishName">The English name of the media.</param>
        /// <param name="imageUrl">The URL of the media's image.</param>
        /// <param name="status">The status of the media.</param>
        /// <param name="pgRating">The age rating of the media.</param>
        /// <param name="description">A description of the media.</param>
        /// <param name="idUserWhoAdded">The ID of the user who added the media.</param>
        /// <param name="timeAdded">The date and time when the media was added.</param>
        /// <param name="type2">The secondary media type.</param>
        /// <param name="studio">The studio that produced the media.</param>
        /// <param name="totalEpisodes">The total number of episodes.</param>
        /// <param name="releasedEpisode">The number of released episodes.</param>
        /// <param name="episodeDuration">The duration of an episode.</param>
        /// <param name="timeUntilNewEpisodeInSeconds">The time until the next episode is released.</param>
        /// <param name="nextEpisodeDateTime">The date and time of the next episode's release.</param>
        /// <param name="startDate">The start date of the media.</param>
        /// <param name="endDate">The end date of the media.</param>
        public Media(int mediaId, MainMediaType type, string originalName, string englishName,
                     string imageUrl, MediaStatus status, PG_Rating pgRating, string description,
                     int? idUserWhoAdded, DateTime? timeAdded, MediaType type2, string studio,
                     int? totalEpisodes, int releasedEpisode, int? episodeDuration,
                     int? timeUntilNewEpisodeInSeconds, DateTime? nextEpisodeDateTime,
                     DateTime? startDate, DateTime? endDate)
        {
            Id = mediaId;
            MainType = type;
            OriginalName = originalName;
            EnglishName = englishName;
            ImageURL = imageUrl;
            Status = status;
            PG_Rating = pgRating;
            Description = description;
            IsAdded = false;
            IDUserWhoAdded = idUserWhoAdded;
            TimeAdded = timeAdded;

            SecondMediaType = type2;
            Studio = studio;
            TotalEpisodes = totalEpisodes;
            ReleasedEpisode = releasedEpisode;
            EpisodeDuration = episodeDuration;
            TimeUntilNewEpisodeInSeconds = timeUntilNewEpisodeInSeconds;
            NextEpisodeDateTime = nextEpisodeDateTime;
            StartDate = startDate;
            EndDate = endDate;
        }

        /// <summary>
        /// Returns a string representation of the episode status in the format "Released / Total".
        /// </summary>
        /// <returns>A string representing the number of released episodes out of the total.</returns>
        public string getEpisodeString()
        {
            return TotalEpisodes == null ? "?" : ReleasedEpisode.ToString() + " / " + TotalEpisodes.ToString();
        }

        /// <summary>
        /// Returns a string representation of the episode duration in minutes.
        /// </summary>
        /// <returns>A string representing the duration of the episode.</returns>
        public string getEpisodeduration()
        {
            return EpisodeDuration == null ? "?" : EpisodeDuration.ToString() + " min";
        }

        /// <summary>
        /// Returns a string representation of the media status, including 
        /// relevant dates if applicable.
        /// </summary>
        /// <returns>A string representing the current status of the media.</returns>
        public string getStatusString()
        {
            if (Status == MediaStatus.Announced ||
                Status == MediaStatus.Delayed ||
                Status == MediaStatus.Canceled) return Status.ToString();
            else if (Status == MediaStatus.Ongoing) return Status.ToString() + " from " + StartDate.Value.ToString("yyyy-MM-dd");

            return EndDate == null ? Status.ToString() : Status.ToString() + " " + EndDate.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Calculates the date of the next episode's release based on the 
        /// start date and the time until the next episode.
        /// </summary>
        /// <returns>A string representing the date of the next episode.</returns>
        public string getNextEpisodeDAte()
        {
            NextEpisodeDateTime = StartDate.Value.AddSeconds((double)TimeUntilNewEpisodeInSeconds * ReleasedEpisode);

            while (NextEpisodeDateTime < DateTime.Now)
            {
                NextEpisodeDateTime = NextEpisodeDateTime.Value.AddSeconds((double)TimeUntilNewEpisodeInSeconds);
            }

            return NextEpisodeDateTime.Value.ToString("yyyy-MM-dd");
        }

        /// <summary>
        /// Returns a string representation of the media object, including 
        /// all relevant properties.
        /// </summary>
        /// <returns>A string representing the media object.</returns>
        public override string ToString()
        {
            return base.ToString() + $", Total Episodes: {TotalEpisodes}, Released Episode: {ReleasedEpisode}, " +
                   $"Episode Duration: {EpisodeDuration}, Time Until New Episode: {TimeUntilNewEpisodeInSeconds}, " +
                   $"Next Episode DateTime: {NextEpisodeDateTime}, Start Date: {StartDate}, End Date: {EndDate}";
        }
    }
}
