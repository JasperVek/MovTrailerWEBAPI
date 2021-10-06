using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using MovieTrailerWEBAPI;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TutorialWebApi.Entities;
using TutorialWebApi.Models;

namespace TutorialWebApi.Controllers
{

	public class MovieController : Controller
	{
		
		private string top10Key = "top10";
		private readonly IOptions<AppSettings> _options;
		private ReposCache cacheFetcher;

		public MovieController(IMemoryCache memoryCache, IOptions<AppSettings> options)
        {
			_options = options;
			cacheFetcher = ReposCache.getInstance(memoryCache, options);
		}

		public IActionResult Index(string search)
        {
			if(search != null)
            {
				string filteredSearch = FilteredSearch(search);
				return View(new MovieViewModel(SearchMovie(filteredSearch)));
			}
			else
            {
				return View(new MovieViewModel(GetTop10()));
			}
        }

        public List<Movie> SearchMovie(string movieTitle)
		{
			List<Movie> cachedMovies = cacheFetcher.GetMoviesFromCache(movieTitle);
			return cachedMovies;
		}

		public List<Movie> GetTop10()
        {
			List<Movie> cachedMovies = cacheFetcher.GetMoviesFromCache(top10Key);
			return cachedMovies;
		}


		private string FilteredSearch(string search)
		{
			string filtered = "";

			Regex reg = new Regex(@"[a-zA-Z0-9]");
			foreach(char c in search)
            {
				if(reg.IsMatch(c.ToString()))
                {
					filtered = filtered + c;
                }
            }
			return filtered;
		}
    }
}
