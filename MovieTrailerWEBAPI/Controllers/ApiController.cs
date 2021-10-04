using Microsoft.AspNetCore.Mvc;
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
        private string imdbKey = "k_oaw79s1y";
        private string youtubeKey = "AIzaSyBGw8lR_VWabVKfCooxSfRS40NcMaU1l14";
        private string searchMovieUrl = "https://imdb-api.com/en/API/SearchMovie/";
        private string top100MovieUrl = "https://imdb-api.com/en/API/MostPopularMovies/";
        private string youtubeTrailerUrl = "https://youtube.googleapis.com/youtube/v3/search?part=snippet&q=";
        private JsonHelper jsonHelper = new JsonHelper();

        public ApiController()
        {
        }

        // Retrieve Movie + Youtube data by Title
        // movies/movieTitle
        [HttpGet("{movieTitle}")]
        public ActionResult<List<Movie>> GetMovie(string movieTitle)
        {
            List<Movie> movieResult = new List<Movie>();

            // client init + search with searchterm
            var client = new RestClient(searchMovieUrl + imdbKey +"/" + movieTitle);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

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
            var client = new RestClient(top100MovieUrl + imdbKey);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

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
            var client = new RestClient(youtubeTrailerUrl + searchTerm + "&key=" + youtubeKey);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.IsSuccessful)
            {
                // gets 5 results, so possibly more trailers to be shown per movie object in the future
                JsonHelper jsonHelper = new JsonHelper();
                List<Youtube> youtubeSearched = jsonHelper.ExtractYoutube(response.Content);

                // return first result
                if (youtubeSearched[0].videoId != null)
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
    }
}
