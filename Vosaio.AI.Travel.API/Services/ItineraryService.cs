using System.Text.Json;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.Data;

namespace Vosaio.AI.Travel.API.Services;

public class ItineraryService : IItineraryService
{
    private readonly IAIIntegrationService _aiIntegrationService;
    private readonly IItineraryRepository _itineraryRepository;
    private readonly ILogger<ItineraryService> _logger;

    public ItineraryService(IAIIntegrationService aiIntegrationService, IItineraryRepository itineraryRepository, ILogger<ItineraryService> logger)
    {
        _aiIntegrationService = aiIntegrationService ?? throw new ArgumentNullException(nameof(aiIntegrationService));
        _itineraryRepository = itineraryRepository ?? throw new ArgumentNullException(nameof(itineraryRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Generates a travel itinerary based on user preferences by integrating with an AI service.
    /// </summary>
    /// <param name="request">The travel request containing destination, dates, budget, and interests.</param>
    /// <returns>A structured itinerary response.</returns>
    public async Task<ItineraryResponse> GenerateItineraryAsync(TravelRequest request)
    {
        _logger.LogInformation("Starting itinerary generation for destination: {Destination}", request.Destination);

        try
        {
            // Call the AI integration service to generate the itinerary.
            var itineraryResponse = await _aiIntegrationService.GetItineraryFromAIAsync(request);

            // Create a record of the itinerary.
            var record = new ItineraryRecord
            {
                Destination = itineraryResponse.Destination,
                StartDate = request.TravelDates[0],
                EndDate = request.TravelDates[1],
                Budget = request.Budget,
                Interests = JsonSerializer.Serialize(request.Interests),
                ItineraryJson = JsonSerializer.Serialize(itineraryResponse.Itinerary),
                CreatedAt = DateTime.UtcNow
            };

            // Save the record using the repository.
            await _itineraryRepository.AddItineraryRecordAsync(record);

            _logger.LogInformation("Itinerary generation completed successfully for destination: {Destination}", request.Destination);

            return itineraryResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating itinerary for request: {@Request}", request);
            throw;
        }
    }
}
