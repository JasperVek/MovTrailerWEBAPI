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
            List<Movie> movieResult = new List<Movie>();

            // client init + search with searchterm
            string url = _options.Value.searchMovieUrl + _options.Value.imdbKey + "/" + movieTitle;
            IRestResponse response = GetRestResponse(url);

            if (response.IsSuccessful)
            {
                JsonHelper jsonHelper = new JsonHelper();
                movieResult = jsonHelper.ExtractMovies(response.Content);

                    // top 5 search results movies + get Youtube link per movie
                    List<Movie> top5List = new List<Movie>();
                    int index = 0;
                    foreach(Movie item in movieResult)
                    {
                        if(index > 5)
                        {
                            break;
                        }

                        Youtube tempYoutube = GetYoutube(item).Value;

                        if (tempYoutube != null)
                        {
                            item.youtubeItem = tempYoutube;
                            top5List.Add(item);
                        }
                        index++;
                    }
                    return top5List;
            }
            else
            {
                return NotFound();
            }
        }

        // Get the top 10 movies with Youtube trailer link
        // largly the same code as GetMovie, but uses another API call to imdb-api
        [HttpGet]
        [Route("movies/top10")]
        public ActionResult<List<Movie>> GetTop10()
        {
            List<Movie> movieResult = new List<Movie>();

            // client init + search with searchterm
            string url = _options.Value.top100MovieUrl + _options.Value.imdbKey;
            IRestResponse response = GetRestResponse(url);

            if (response.IsSuccessful)
            {
                JsonHelper jsonHelper = new JsonHelper();
                movieResult = jsonHelper.ExtractMovies(response.Content);

                    // top 10 search results
                    List<Movie> top10List = new List<Movie>();
                    int index = 0;
                    foreach (Movie item in movieResult)
                    {
                        // stop with max 10 items
                        if (index > 10)
                        {
                            break;
                        }

                        Youtube tempYoutube = GetYoutube(item).Value;

                        if (tempYoutube != null) { item.youtubeItem = tempYoutube; }
                        top10List.Add(item);

                        index++;
                    }
                return top10List;
                
            }
            else
            {
                return NotFound();
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
    }
}
