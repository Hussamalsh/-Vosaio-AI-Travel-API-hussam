using Vosaio.AI.Travel.API.Models;

namespace Vosaio.AI.Travel.API.OpenAI;

/// <summary>
/// Constructs the prompt text to be sent to the AI service.
/// </summary>
public class PromptBuilder : IPromptBuilder
{
    public string BuildPrompt(TravelRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.TravelDates == null || request.TravelDates.Count != 2)
            throw new ArgumentException("TravelDates must contain exactly two dates.", nameof(request));

        // Generating the prompt using the request data.
        return $@"
Generate a travel itinerary for {request.Destination}
from {request.TravelDates[0]:yyyy-MM-dd} to {request.TravelDates[1]:yyyy-MM-dd}
with a budget of {request.Budget} and interests in {string.Join(", ", request.Interests)}.
Return a valid JSON object that exactly follows the provided schema, and include only
the start and end dates in the 'TravelDates' array. Do not include any extra text or commentary."
            .Trim();
    }
}
