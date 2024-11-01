namespace MediaNexus_Backend
{
    /// <summary>
    /// Represents a genre of media in the Media Nexus application.
    /// </summary>
    public class Genres
    {
        /// <summary>
        /// Gets or sets the unique identifier for the genre.
        /// </summary>
        public int GenreID { get; set; }

        /// <summary>
        /// Gets or sets the name of the genre.
        /// </summary>
        public string GenreName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Genres"/> class with the specified genre ID and name.
        /// </summary>
        /// <param name="genreID">The unique identifier for the genre.</param>
        /// <param name="genreName">The name of the genre.</param>
        public Genres(int genreID, string genreName)
        {
            GenreID = genreID;
            GenreName = genreName;
        }

        /// <summary>
        /// Returns a string that represents the current genre.
        /// </summary>
        /// <returns>The name of the genre.</returns>
        public override string ToString()
        {
            return GenreName;
        }
    }
}
