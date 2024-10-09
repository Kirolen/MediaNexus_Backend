using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public class Book : MainMedia
    {
        public string Author { get; set; }
        public DateTime? PublicationDate { get; set; }
        public int Pages { get; set; }
        public string ISBN { get; set; }

        public Book() { }

        public Book(int mediaId, MainMediaType type, string originalName, string englishName,
                     string imageUrl, MediaStatus status, PG_Rating pgRating, string description,
                     int? idUserWhoAdded, DateTime? timeAdded, string author, DateTime publicationDate,
                     int pages, string isbn)
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

            Author = author;
            PublicationDate = publicationDate;
            Pages = pages;
            ISBN = isbn;
        }

        public override string ToString()
        {
            return base.ToString() + $", Author: {Author}, Publication Date: {PublicationDate}, " +
                   $"Pages: {Pages}, ISBN: {ISBN}";
        }
    }
}
