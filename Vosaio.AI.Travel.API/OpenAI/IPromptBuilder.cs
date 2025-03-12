using Vosaio.AI.Travel.API.Models;

namespace Vosaio.AI.Travel.API.OpenAI;

/// <summary>
/// Defines a contract for building prompts based on a travel request.
/// </summary>
public interface IPromptBuilder
{
    /// <summary>
    /// Builds the prompt text for the AI service based on the provided travel request.
    /// </summary>
    /// <param name="request">The travel request containing destination, dates, budget, and interests.</param>
    /// <returns>The constructed prompt string.</returns>
    string BuildPrompt(TravelRequest request);
}
