using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using StarWarsAPI.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace StarWarsAPI.Services
{
    public class StarshipService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<StarshipService> _logger;

        public StarshipService(IHttpClientFactory httpClientFactory, ILogger<StarshipService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<List<Starship>> GetStarshipsAsync(string? manufacturer, int page = 1, int limit = 10)
        {
            var client = _httpClientFactory.CreateClient();

            // Construct the paginated URL
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

            return starships.Take(limit).ToList();  // Return the limited number of results
        }

    }
}
