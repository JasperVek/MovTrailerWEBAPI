using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MovieTrailerWEBAPI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorialWebApi.Entities;

namespace TutorialWebApi.Controllers
{

    [ApiController]
    [Route("movies")]
    public class ApiController : ControllerBase
    {
        private JsonHelper jsonHelper = new JsonHelper();
        private readonly IOptions<AppSettings> _options;

        public ApiController(IOptions<AppSettings> options)
        {
            _options = options;
        }

        // Retrieve Movie + Youtube data by Title
        // movies/movieTitle
        [HttpGet("{movieTitle}")]
        public ActionResult<List<Movie>> GetMovie(string movieTitle)
        {
            try
            {
                string url = _options.Value.searchMovieUrl + _options.Value.imdbKey + "/" + movieTitle;
                return FetchMovies(5, url);
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        // Get the top 10 movies with Youtube trailer link
        // largly the same code as GetMovie, but uses another API call to imdb-api
        [HttpGet]
        [Route("movies/top10")]
        public ActionResult<List<Movie>> GetTop10()
        {
            try
            {
                string url = _options.Value.top100MovieUrl + _options.Value.imdbKey;
                return FetchMovies(10, url);
            }
            catch
            {
                throw new ArgumentException();
            }

        }


        // Retrieve Youtube trailer results via Youtube API, with the first result as return 
            public ActionResult<Youtube> GetYoutube(Movie movie)
        {
            Youtube youtubeResult = new Youtube();
            youtubeResult.videoId = "";
            string searchTerm = movie.title + movie.description + " official trailer";
            string url = _options.Value.youtubeTrailerUrl + searchTerm + "&key=" + _options.Value.youtubeKey;
            IRestResponse response = GetRestResponse(url);

            if (response.IsSuccessful)
            {
                // gets 5 results, so possibly more trailers to be shown per movie object in the future
                JsonHelper jsonHelper = new JsonHelper();
                List<Youtube> youtubeSearched = jsonHelper.ExtractYoutube(response.Content);

                // return first result
                if (youtubeSearched != null && youtubeSearched[0].videoId != null)
                { 
                youtubeResult = youtubeSearched[0];
                }
            }
            else
            {
                return youtubeResult;
            }
            return youtubeResult;
        }

        private IRestResponse GetRestResponse(string url)
        {
            var client = new RestClient(url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            return response;
        }

        private List<Movie> FetchMovies(int maxReturn, string url)
        {
            List<Movie> movieOutput = new List<Movie>();

            IRestResponse response = GetRestResponse(url);

            if (response.IsSuccessful)
            {
                JsonHelper jsonHelper = new JsonHelper();
                List<Movie> movieResult = new List<Movie>();
                movieResult = jsonHelper.ExtractMovies(response.Content);

                // how many movies to return max
                var toReturnMovies = movieResult.Take<Movie>(maxReturn);

                foreach (Movie item in toReturnMovies)
                {
                    Youtube tempYoutube = GetYoutube(item).Value;
                    if (tempYoutube != null) { item.youtubeItem = tempYoutube; }
                    movieOutput.Add(item);
                }
            }
            else
            {
                return null;
            }
            return movieOutput;
        }
    }
}
