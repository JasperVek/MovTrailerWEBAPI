using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorialWebApi.Entities;

namespace TutorialWebApi.Models
{
    public class MovieViewModel
    {
        public List<Movie> MovieResults { get; set; }

        public MovieViewModel(List<Movie> movies)
        {
            this.MovieResults = movies;
        }
    }
}
