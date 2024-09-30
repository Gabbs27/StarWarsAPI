using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StarWarsAPI.Services;

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
    public async Task<IActionResult> GetStarships([FromQuery] string? manufacturer, [FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        try
        {
            var starships = await _starshipService.GetStarshipsAsync(manufacturer, page, limit);

            if (starships.Count == 0)
            {
                return NoContent();  // Return 204 No Content if no starships found
            }

            return Ok(starships);
        }
        catch (HttpRequestException ex)
        {
            // Return 502 Bad Gateway if there's an issue with the external API
            return StatusCode(502, new { message = "Error fetching starships from the external API. Please try again later.", details = ex.Message });
        }
        catch (Exception ex)
        {
            // Return 500 Internal Server Error for any unhandled exceptions
            return StatusCode(500, new { message = "An unexpected error occurred. Please try again later.", details = ex.Message });
        }
    }
}
