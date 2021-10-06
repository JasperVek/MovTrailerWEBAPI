using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TutorialWebApi.Entities
{
    public class Movie
    {
        public Movie()
        {
            youtubeItem = null;
        }
        public string id { get; set; }

        public string image { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public string year { get; set; }

        public Youtube youtubeItem { get; set; }

    }
}
