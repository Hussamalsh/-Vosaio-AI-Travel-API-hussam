using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using System.Text.Json;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.OpenAI;

namespace Vosaio.AI.Travel.API.Services;

/// <summary>
/// Integrates with OpenAI to generate travel itineraries based on user preferences.
/// </summary>
public class AIIntegrationService : IAIIntegrationService
{
    private readonly IOpenAIClient _openAIClient;
    private readonly IPromptBuilder _promptBuilder;
    private readonly ILogger<AIIntegrationService> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    private static readonly string _schemaString = ItinerarySchema.Value;

    public AIIntegrationService(IOpenAIClient openAIClient, IPromptBuilder promptBuilder, ILogger<AIIntegrationService> logger)
    {
        _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _promptBuilder = promptBuilder ?? throw new ArgumentNullException(nameof(promptBuilder));
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Uses the OpenAI-DotNet package to generate a travel itinerary based on the provided travel request.
    /// </summary>
    /// <param name="request">The travel request including destination, travel dates, budget, and interests.</param>
    /// <returns>An <see cref="ItineraryResponse"/> containing structured itinerary details.</returns>
    public async Task<ItineraryResponse> GetItineraryFromAIAsync(TravelRequest request)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        if (request.TravelDates == null || request.TravelDates.Count != 2)
            throw new ArgumentException("Request must contain exactly two travel dates.", nameof(request));


        var jsonSchema = new JsonSchema("itinerary_response", _schemaString, strict: true);

        // Build the user prompt for the AI model.
        string prompt = _promptBuilder.BuildPrompt(request);

        _logger.LogInformation("Sending prompt to OpenAI: {Prompt}", prompt);

        var messages = new List<Message>
        {
            new(Role.System, "You are a travel itinerary generator. Respond only with valid JSON that conforms to the given schema."),
            new(Role.User, prompt)
        };

        var chatRequest = new ChatRequest(
            messages,
            model: Model.GPT4o,
            temperature: 0.7,
            maxTokens: 500,
            jsonSchema: jsonSchema
        );

        _logger.LogInformation("Sending chat request to OpenAI.");

        try
        {
            var chatResponse = await _openAIClient.ChatEndpoint.GetCompletionAsync(chatRequest);

            // Validate the response from OpenAI.
            if (chatResponse?.Choices is not { Count: > 0 })
            {
                _logger.LogError("No response received from OpenAI chat endpoint.");
                throw new InvalidOperationException("AI service failed to generate a response.");
            }

            // Extract the assistant's message content.
            var contentElement = chatResponse.Choices[0].Message.Content;
            var aiResponseText = contentElement.ValueKind == JsonValueKind.String
                ? contentElement.GetString()?.Trim()
                : contentElement.ToString();

            if (string.IsNullOrWhiteSpace(aiResponseText))
            {
                _logger.LogError("The AI response text was empty or null.");
                throw new InvalidOperationException("AI service returned an empty itinerary.");
            }

            _logger.LogInformation("Received response from OpenAI: {ResponseText}", (object)aiResponseText);

            // Deserialize into our ItineraryResponse model.
            ItineraryResponse? itineraryResponse;
            try
            {
                itineraryResponse = JsonSerializer.Deserialize<ItineraryResponse>(
                    aiResponseText,
                    _jsonSerializerOptions
                );
            }
            catch (JsonException jsonEx)
            {
                _logger.LogError(jsonEx, "Failed to deserialize AI response as JSON. Raw response: {RawResponse}",(object) aiResponseText);
                throw new InvalidOperationException("Failed to parse AI itinerary response.", jsonEx);
            }

            if (itineraryResponse is null)
            {
                _logger.LogError("Deserialization of the AI response returned null. Raw response: {RawResponse}", (object)aiResponseText);
                throw new InvalidOperationException("Deserialization returned null.");
            }

            return itineraryResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during call to OpenAI ChatEndpoint with request: {ChatRequest}", chatRequest);
            throw new ApplicationException("An error occurred while generating the itinerary. Please try again later.", ex);
        }
    }
}