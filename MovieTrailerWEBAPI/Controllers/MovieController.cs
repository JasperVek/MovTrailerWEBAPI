﻿using Microsoft.AspNetCore.Mvc;
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
		
		private IMemoryCache cache;
		private string top10Key = "top10";
		private readonly IOptions<AppSettings> _options;

		public MovieController(IMemoryCache memoryCache, IOptions<AppSettings> options)
        {
			cache = memoryCache;
			_options = options;
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
			List<Movie> cachedMovies = GetMoviesFromCache(movieTitle);
			return cachedMovies;
		}

		public List<Movie> GetTop10()
        {
			List<Movie> cachedMovies = GetMoviesFromCache(top10Key);
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


		// key for cache is top10 for GetTop10, or movieTitle for a search title
		private List<Movie> GetMoviesFromCache(string key)
        {
			ApiController _apiController = new ApiController(_options);
			List<Movie> movieCache;
			if (!cache.TryGetValue(key, out movieCache))
			{
				if(key == top10Key)
                {
					movieCache = _apiController.GetTop10().Value;
				}
                else
                {
					movieCache = _apiController.GetMovie(key).Value;
				}
				AddMovieToCache(key, movieCache);
			}
			return movieCache;
		}

		private void AddMovieToCache(string key, List<Movie> movies)
        {
			var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
			cache.Set<List<Movie>>(key, movies, cacheOptions);
		}
    }
}
