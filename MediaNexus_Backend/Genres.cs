using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaNexus_Backend
{
    public class Genres
    {
        public int GenreID { get; set; }
        public string GenreName { get; set; }

        public Genres(int genreID, string genreName)
        {
            GenreID = genreID;
            GenreName = genreName;
        }

        public override string ToString()
        {
            return GenreName;
        }
    }
}
