using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public enum MainMediaType
    {
        Book,
        Media,
        Game,
        Comics,
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


    public class MainMedia
    {
        public int Id { get; set; }
        public MainMediaType MainType { get; set; }
        public string OriginalName { get; set; }
        public string EnglishName { get; set; }
        public string ImageURL { get; set; }
        public MediaStatus Status { get; set; }
        public PG_Rating PG_Rating { get; set; }
        public string Description { get; set; }
        public bool? IsAdded { get; set; }
        public int? IDUserWhoAdded { get; set; }
        public DateTime? TimeAdded { get; set; }


        public MainMedia() { }

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
