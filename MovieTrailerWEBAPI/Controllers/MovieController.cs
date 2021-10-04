using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TutorialWebApi.Entities;
using TutorialWebApi.Models;

namespace TutorialWebApi.Controllers
{

	public class MovieController : Controller
	{
		private ApiController apiController = new ApiController();
		private IMemoryCache cache;
		private string top10Key = "top10";

		public MovieController(IMemoryCache memoryCache)
        {
			cache = memoryCache;
        }

		public IActionResult Index(string search)
        {
			if(search != null)
            {
				return View(new MovieViewModel(SearchMovie(search)));
			}
			else
            {
				return View(new MovieViewModel(GetTop10()));
			}
        }

		public List<Movie> SearchMovie(string movieTitle)
		{
			var movies = apiController.GetMovie(movieTitle);
			return movies.Value;
		}

		public List<Movie> GetTop10()
        {
			return apiController.GetTop10().Value;
		}
    }
}
