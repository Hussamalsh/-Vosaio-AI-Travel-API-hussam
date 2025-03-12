using Microsoft.AspNetCore.Mvc;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.Services;

namespace Vosaio.AI.Travel.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItineraryController : ControllerBase
{
    private readonly IItineraryService _itineraryService;
    private readonly ILogger<ItineraryController> _logger;

    public ItineraryController(IItineraryService itineraryService, ILogger<ItineraryController> logger)
    {
        _itineraryService = itineraryService ?? throw new ArgumentNullException(nameof(itineraryService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a travel itinerary based on user preferences.
    /// </summary>
    /// <param name="request">The travel request details.</param>
    /// <returns>A structured itinerary in JSON format.</returns>
    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] TravelRequest request)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid travel request received: {@Request}", request);
            return BadRequest(ModelState);
        }

        try
        {
            // the service responsible for generating the itinerary.
            var itineraryResponse = await _itineraryService.GenerateItineraryAsync(request);
            return Ok(itineraryResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while generating itinerary for request: {@Request}", request);
            return StatusCode(500, new { Message = "An error occurred while processing your request. Please try again later." });
        }
    }
}
