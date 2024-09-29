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
            string cacheKey = $"Starships_Page_{page}_Limit_{limit}";

            if (!_cache.TryGetValue(cacheKey, out List<Starship> cachedStarships))
            {
                var client = _httpClientFactory.CreateClient();
                var url = $"https://swapi.dev/api/starships/?page={page}";

                var response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException("Error fetching starships");
                }

                var starshipsData = await response.Content.ReadAsStringAsync();
                var starshipsResponse = JsonConvert.DeserializeObject<StarshipResponse>(starshipsData);

                var starships = starshipsResponse?.Results ?? new List<Starship>();

                // Filter by manufacturer
                if (!string.IsNullOrEmpty(manufacturer))
                {
                    string normalizedManufacturer = manufacturer.Trim().ToLower();
                    starships = starships
                        .Where(s => s.Manufacturer.ToLower().Contains(normalizedManufacturer))
                        .ToList();
                }

                // Cache the starships for 10 minutes
                _cache.Set(cacheKey, starships.Take(limit).ToList(), TimeSpan.FromMinutes(10));
                return starships;
            }

            // Return cached starships if they exist
            return cachedStarships;
        }
    }
}
