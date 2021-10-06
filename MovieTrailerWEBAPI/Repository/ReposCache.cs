using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TutorialWebApi.Controllers;
using TutorialWebApi.Entities;

namespace MovieTrailerWEBAPI
{
    public class ReposCache
    {
		private static ReposCache uniqueInstance;

		private IMemoryCache cache;
		private const string top10Key = "top10";
		private readonly IOptions<AppSettings> _options;

		private ReposCache(IMemoryCache memoryCache, IOptions<AppSettings> options)
        {
			cache = memoryCache;
			_options = options;
		}

		public static ReposCache getInstance(IMemoryCache memoryCache, IOptions<AppSettings> options)
        {
			if(uniqueInstance == null)
            {
				uniqueInstance = new ReposCache(memoryCache, options);
            }
			return uniqueInstance;
        }


		public List<Movie> GetMoviesFromCache(string key, ApiController? apiController = null)
		{

			ApiController _apiController;
			List<Movie> movieCache;

			movieCache = CheckCache(key);
			if(movieCache != null)
            {
				return movieCache;
            }
            else
            {
				if (apiController != null)
				{
					_apiController = apiController;
				}
				else
				{
					_apiController = new ApiController(cache, _options);
				}

				movieCache = GetMovieForCache(key, _apiController);
			}

			return movieCache;
		}

		public List<Movie> CheckCache(string key)
        {
			List<Movie> movieCache;

			if (cache.TryGetValue(key, out movieCache))
			{
				return movieCache;
			}
			else
            {
				return null;
			}
		}

		// key for cache is top10 for GetTop10, or movieTitle for a search title
		private List<Movie> GetMovieForCache(string key, ApiController _apiController)
        {
			List<Movie> movieCache;
			if (key == top10Key)
			{
				movieCache = _apiController.FetchTop10();
			}
			else
			{
				movieCache = _apiController.FetchMovies(key);
			}

			if(movieCache != null)
            {
				AddMovieToCache(key, movieCache);
				return movieCache;
			}
            else
            {
				return null;
            }
		}

		private void AddMovieToCache(string key, List<Movie> movies)
		{
			var cacheOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1));
			cache.Set<List<Movie>>(key, movies, cacheOptions);
		}
	}
}
