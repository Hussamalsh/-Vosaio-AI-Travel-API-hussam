using Vosaio.AI.Travel.API.Models;

namespace Vosaio.AI.Travel.API.Services;

/// <summary>
/// Provides an abstraction for AI integration to generate travel itineraries.
/// </summary>
public interface IAIIntegrationService
{
    /// <summary>
    /// Generates a travel itinerary using an AI service based on the provided travel request.
    /// </summary>
    /// <param name="request">The travel request containing destination, travel dates, budget, and interests.</param>
    /// <returns>A task that represents the asynchronous operation, containing the generated itinerary response.</returns>
    Task<ItineraryResponse> GetItineraryFromAIAsync(TravelRequest request);
}
