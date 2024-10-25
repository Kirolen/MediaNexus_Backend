using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public class SortMediacs
    {
        public string[] selectedTypes = Array.Empty<string>();
        public Genres[] selectedGenres = Array.Empty<Genres>();
        public string[] selectedStatus = Array.Empty<string>();
        public int Total;

        public SortMediacs(string[] Types, Genres[] genres, string[] status)
        {
            selectedTypes = Types;
            selectedGenres = genres;
            selectedStatus = status;
        }

        public SortMediacs() { }
    }
}
