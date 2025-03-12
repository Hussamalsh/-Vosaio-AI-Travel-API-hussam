using Vosaio.AI.Travel.API.Models;

namespace Vosaio.AI.Travel.API.Services;

public interface IItineraryService
{
    Task<ItineraryResponse> GenerateItineraryAsync(TravelRequest request);
}
