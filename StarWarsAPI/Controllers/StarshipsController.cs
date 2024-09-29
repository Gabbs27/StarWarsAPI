using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsAPI.Services;
using System.Threading.Tasks;

namespace StarWarsAPIChallenge.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StarshipsController : ControllerBase
    {
        private readonly StarshipService _starshipService;

        public StarshipsController(StarshipService starshipService)
        {
            _starshipService = starshipService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetStarships([FromQuery] string? manufacturer)
        {
            var starships = await _starshipService.GetStarshipsAsync(manufacturer);

            if (starships.Count == 0)
            {
                return NoContent();  
            }

            return Ok(starships);  
        }
    }
}
