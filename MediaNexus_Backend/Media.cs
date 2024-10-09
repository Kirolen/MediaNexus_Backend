using System;

namespace MediaNexus_Backend
{
    public enum MediaType
    {
        Movie,
        Serial
    }
    public class Media : MainMedia
    {
        public MediaType SecondMediaType { get; set; }
        public string Studio { get; set; }
        public int TotalEpisodes { get; set; }                 
        public int ReleasedEpisode { get; set; }              
        public int EpisodeDuration { get; set; }              
        public int? TimeUntilNewEpisodeInSeconds { get; set; } 
        public DateTime? NextEpisodeDateTime { get; set; }     
        public DateTime? StartDate { get; set; }              
        public DateTime? EndDate { get; set; }                  

        public Media() { }

        public Media(int mediaId, MainMediaType type, string originalName, string englishName,
                     string imageUrl, MediaStatus status, PG_Rating pgRating, string description,
                     int? idUserWhoAdded, DateTime? timeAdded, MediaType type2, string studio,
                     int totalEpisodes, int releasedEpisode, int episodeDuration,
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

        public override string ToString()
        {
            return base.ToString() + $", Total Episodes: {TotalEpisodes}, Released Episode: {ReleasedEpisode}, " +
                   $"Episode Duration: {EpisodeDuration}, Time Until New Episode: {TimeUntilNewEpisodeInSeconds}, " +
                   $"Next Episode DateTime: {NextEpisodeDateTime}, Start Date: {StartDate}, End Date: {EndDate}";
        }
    }
}

