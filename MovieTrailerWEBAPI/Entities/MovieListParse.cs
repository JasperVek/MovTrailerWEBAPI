using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorialWebApi.Entities;

namespace MovieTrailerWEBAPI.Entities
{
    public class MovieListParse
    {
        public List<Movie> items { get; set; }

        public List<Movie> results { get; set; }
    }
}
