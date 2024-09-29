using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using StarWarsAPIChallenge.Models;
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

        public async Task<List<Starship>> GetStarshipsAsync(string? manufacturer)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync("https://swapi.dev/api/starships/");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException("Error fetching starships");
            }

            var starshipsData = await response.Content.ReadAsStringAsync();


            var starshipsResponse = JsonConvert.DeserializeObject<StarshipResponse>(starshipsData);


            var starships = starshipsResponse?.Results ?? new List<Starship>();


            if (!string.IsNullOrEmpty(manufacturer))
            {
                string normalizedManufacturer = manufacturer.Trim().ToLower();

                starships = starships
                    .Where(s => s.Manufacturer.ToLower().Contains(normalizedManufacturer))
                    .ToList();
            }

            return starships;
        }
    }
}
