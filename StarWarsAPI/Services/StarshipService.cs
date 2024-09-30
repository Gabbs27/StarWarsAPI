using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using StarWarsAPI.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace StarWarsAPI.Services
{
    public class StarshipService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StarshipService> _logger;
        private readonly IMemoryCache _cache;

        public StarshipService(IHttpClientFactory httpClientFactory, ILogger<StarshipService> logger, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _cache = cache;
        }

        public async Task<List<Starship>> GetStarshipsAsync(string? manufacturer, int page = 1, int limit = 10)
        {
            string cacheKey = $"starships_{manufacturer}_{page}_{limit}";
            if (_cache.TryGetValue(cacheKey, out List<Starship> cachedStarships))
            {
                return cachedStarships;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync($"https://swapi.dev/api/starships/?page={page}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Error fetching starships from external API");
                }

                var starshipsData = await response.Content.ReadAsStringAsync();
                var starshipsResponse = JsonConvert.DeserializeObject<StarshipResponse>(starshipsData);

                var starships = starshipsResponse?.Results ?? new List<Starship>();

                if (!string.IsNullOrEmpty(manufacturer))
                {
                    starships = starships
                        .Where(s => s.Manufacturer.ToLower().Contains(manufacturer.Trim().ToLower()))
                        .ToList();
                }

                _cache.Set(cacheKey, starships);

                return starships;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error occurred while fetching starships");
                throw;  // Re-throw to let the test catch the exception
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching starships data");
                throw new Exception("An error occurred while fetching starships data. Please try again later.");
            }
        }
    }
}
