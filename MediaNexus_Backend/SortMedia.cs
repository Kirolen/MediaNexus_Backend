using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public class SortMedia
    {
        public string[] selectedTypes = Array.Empty<string>();
        public Genres[] selectedGenres = Array.Empty<Genres>();
        public string[] selectedStatus = Array.Empty<string>();
        public string[] selectedMediaStatus = Array.Empty<string>();
        public int Total;
        public int userID;

        public SortMedia(string[] Types, Genres[] genres, string[] status, string[] mediaStatus, int userId)
        {
            selectedTypes = Types;
            selectedGenres = genres;
            selectedStatus = status;
            selectedMediaStatus = mediaStatus;
            userID = userId;
        }

        public SortMedia() { }
    }
}
