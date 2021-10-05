using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTrailerWEBAPI
{
    public class AppSettings
    {
        public const string SectionName = "Settings";

        public string searchMovieUrl { get; set; }
        public string top100MovieUrl { get; set; }
        public string youtubeTrailerUrl { get; set; }

        public string imdbKey { get; set; }
        public string youtubeKey { get; set; }
    }
}
