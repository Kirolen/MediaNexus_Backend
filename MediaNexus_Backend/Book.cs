using System;

namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents a book in the Media Nexus application.
    /// </summary>
    public class Book : MainMedia
    {
        /// <summary>
        /// Gets or sets the author of the book.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the publication date of the book.
        /// </summary>
        public DateTime? PublicationDate { get; set; }

        /// <summary>
        /// Gets or sets the number of pages in the book.
        /// </summary>
        public int Pages { get; set; }

        /// <summary>
        /// Gets or sets the ISBN (International Standard Book Number) of the book.
        /// </summary>
        public string ISBN { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class.
        /// </summary>
        public Book() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Book"/> class with the specified details.
        /// </summary>
        /// <param name="mediaId">The unique identifier for the media.</param>
        /// <param name="type">The main media type.</param>
        /// <param name="originalName">The original name of the book.</param>
        /// <param name="englishName">The English name of the book.</param>
        /// <param name="imageUrl">The URL of the book's image.</param>
        /// <param name="status">The status of the book.</param>
        /// <param name="pgRating">The PG rating of the book.</param>
        /// <param name="description">The description of the book.</param>
        /// <param name="idUserWhoAdded">The ID of the user who added the book.</param>
        /// <param name="timeAdded">The time the book was added.</param>
        /// <param name="author">The author of the book.</param>
        /// <param name="publicationDate">The publication date of the book.</param>
        /// <param name="pages">The number of pages in the book.</param>
        /// <param name="isbn">The ISBN of the book.</param>
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

        /// <summary>
        /// Returns a string that represents the current book.
        /// </summary>
        /// <returns>A string containing details about the book.</returns>
        public override string ToString()
        {
            return base.ToString() + $", Author: {Author}, Publication Date: {PublicationDate}, " +
                   $"Pages: {Pages}, ISBN: {ISBN}";
        }
    }
}
