using System;

namespace MediaNexus_Backend
{
    public class Media
    {
        public int MediaId { get; set; }
        public string OriginalName { get; set; }
        public string EnglishName { get; set; }
        public string ImageTitle { get; set; }
        public MediaTypeEnum MediaType { get; set; }


        public DateTime ReleaseDate { get; set; }
        public string Studio { get; set; }
        public string Description { get; set; }
        public RatingEnum Rating { get; set; }
        public float SeriesRating { get; set; }
        public StatusEnum Status { get; set; }
        public TimeSpan EpisodeDuration { get; set; }
        public int TotalEpisodes { get; set; }
        public int ReleasedEpisodes { get; set; }
        public DateTime? NextEpisode { get; set; }
        public int TimeUntilNextEpisode { get; set; }
        public bool IsAdded { get; set; }
    }

    public enum MediaTypeEnum
    {
        TV_Show,
        Anime,
        Movie,
        Dorama
    }

    public enum RatingEnum
    {
        G,
        PG,
        PG_13,
        R,
        NC_17
    }

    public enum StatusEnum
    {
        Ongoing,
        Delayed,
        Finished,
        Announced
    }
}
