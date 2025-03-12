using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using OpenAI;
using OpenAI.Chat;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.OpenAI;
using Vosaio.AI.Travel.API.Services;

namespace Vosaio.AI.Travel.API.Tests.Services;

[TestFixture]
public class AIIntegrationServiceTests
{
    private Mock<IOpenAIClient> _openAIClientMock;
    private Mock<IChatEndpoint> _chatEndpointMock;
    private Mock<ILogger<AIIntegrationService>> _loggerMock;
    private AIIntegrationService _service;

    [SetUp]
    public void Setup()
    {
        _openAIClientMock = new Mock<IOpenAIClient>(MockBehavior.Strict);
        _chatEndpointMock = new Mock<IChatEndpoint>(MockBehavior.Strict);
        // Setup the ChatEndpoint property to return our mock endpoint.
        _openAIClientMock.Setup(x => x.ChatEndpoint).Returns(_chatEndpointMock.Object);

        _loggerMock = new Mock<ILogger<AIIntegrationService>>();
        // Supply a concrete PromptBuilder instance along with the logger.
        _service = new AIIntegrationService(_openAIClientMock.Object, new PromptBuilder(), _loggerMock.Object);
    }

    [Test]
    public void GetItineraryFromAIAsync_NullRequest_ThrowsArgumentNullException()
    {
        Assert.ThrowsAsync<ArgumentNullException>(() => _service.GetItineraryFromAIAsync(null));
    }

    [Test]
    public void GetItineraryFromAIAsync_InvalidTravelDates_ThrowsArgumentException()
    {
        var request = new TravelRequest
        {
            Destination = "Tokyo",
            TravelDates = new List<DateTime> { DateTime.Now } // Only one date provided
        };

        Assert.ThrowsAsync<ArgumentException>(() => _service.GetItineraryFromAIAsync(request));
    }

    [Test]
    public void GetItineraryFromAIAsync_NoChoices_ThrowsInvalidOperationException()
    {
        // Arrange: Create a valid travel request.
        var request = CreateValidTravelRequest();

        // Arrange: simulate a ChatResponse with no choices.
        var chatResponse = new ChatResponse();
        SetPrivateField(chatResponse, "choices", new List<Choice>());

        _chatEndpointMock
            .Setup(x => x.GetCompletionAsync(It.IsAny<ChatRequest>()))
            .ReturnsAsync(chatResponse);

        // Act & Assert: Expect an ApplicationException wrapping an InvalidOperationException.
        var ex = Assert.ThrowsAsync<ApplicationException>(() => _service.GetItineraryFromAIAsync(request));
        Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.That(ex.InnerException.Message, Does.Contain("AI service failed to generate a response"));
    }

    [Test]
    public void GetItineraryFromAIAsync_EmptyMessageContent_ThrowsInvalidOperationException()
    {
        // Arrange: Create a valid travel request.
        var request = CreateValidTravelRequest();

        // Arrange: simulate a ChatResponse with empty (whitespace) message content.
        var emptyContent = JsonDocument.Parse("\"   \"").RootElement;
        var choice = new Choice();
        var message = CreateMessageWithContent(Role.Assistant, emptyContent);
        SetPrivateProperty(choice, "Message", message);

        var chatResponse = new ChatResponse();
        SetPrivateField(chatResponse, "choices", new List<Choice> { choice });

        _chatEndpointMock
            .Setup(x => x.GetCompletionAsync(It.IsAny<ChatRequest>()))
            .ReturnsAsync(chatResponse);

        // Act & Assert: Expect an ApplicationException wrapping an InvalidOperationException.
        var ex = Assert.ThrowsAsync<ApplicationException>(() => _service.GetItineraryFromAIAsync(request));
        Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.That(ex.InnerException.Message, Does.Contain("AI service returned an empty itinerary"));
    }

    [Test]
    public void GetItineraryFromAIAsync_InvalidJson_ThrowsInvalidOperationException()
    {
        // Arrange: Create a valid travel request.
        var request = CreateValidTravelRequest();

        // Arrange: simulate a ChatResponse with JSON that doesn't match the expected schema.
        var invalidJson = JsonDocument.Parse("\"Not a valid JSON object\"").RootElement;
        var choice = new Choice();
        var message = CreateMessageWithContent(Role.Assistant, invalidJson);
        SetPrivateProperty(choice, "Message", message);

        var chatResponse = new ChatResponse();
        SetPrivateField(chatResponse, "choices", new List<Choice> { choice });

        _chatEndpointMock
            .Setup(x => x.GetCompletionAsync(It.IsAny<ChatRequest>()))
            .ReturnsAsync(chatResponse);

        // Act & Assert: Expect an ApplicationException wrapping an InvalidOperationException.
        var ex = Assert.ThrowsAsync<ApplicationException>(() => _service.GetItineraryFromAIAsync(request));
        Assert.That(ex.InnerException, Is.TypeOf<InvalidOperationException>());
        Assert.That(ex.InnerException.Message, Does.Contain("Failed to parse AI itinerary response"));
    }

    [Test]
    public async Task GetItineraryFromAIAsync_ValidResponse_ReturnsItineraryResponse()
    {
        // Arrange: Create a valid travel request.
        var request = CreateValidTravelRequest();

        // Arrange: create valid JSON matching the expected schema.
        var validJson = @"{
                ""Destination"": ""Tokyo"",
                ""TravelDates"": [""2025-06-01"", ""2025-06-10""],
                ""Itinerary"": {
                    ""Hotels"": [],
                    ""Activities"": [],
                    ""Restaurants"": [],
                    ""TotalEstimatedCost"": 0
                }
            }";
        var validJsonElement = JsonDocument.Parse(validJson).RootElement;
        var choice = new Choice();
        var message = CreateMessageWithContent(Role.Assistant, validJsonElement);
        SetPrivateProperty(choice, "Message", message);

        var chatResponse = new ChatResponse();
        SetPrivateField(chatResponse, "choices", new List<Choice> { choice });

        _chatEndpointMock
            .Setup(x => x.GetCompletionAsync(It.IsAny<ChatRequest>()))
            .ReturnsAsync(chatResponse);

        // Act
        var result = await _service.GetItineraryFromAIAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.AreEqual("Tokyo", result.Destination);
        Assert.That(result.TravelDates, Is.EquivalentTo(request.TravelDates));
        Assert.NotNull(result.Itinerary);
    }

    /// <summary>
    /// Helper method to create a valid travel request.
    /// </summary>
    private static TravelRequest CreateValidTravelRequest()
    {
        return new TravelRequest
        {
            Destination = "Tokyo",
            TravelDates = new List<DateTime>
            {
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 10)
            },
            Budget = 2000,
            Interests = new List<string> { "history", "food", "adventure" }
        };
    }

    /// <summary>
    /// Creates an uninitialized instance of Message and sets its internal Content property.
    /// </summary>
    private static Message CreateMessageWithContent(Role role, JsonElement content)
    {
        // Create an uninitialized instance of Message.
        var message = (Message)FormatterServices.GetUninitializedObject(typeof(Message));
        // Set the Role property.
        SetPrivateProperty(message, "Role", role);
        // Set the Content property to the provided JsonElement.
        SetPrivateProperty(message, "Content", content);
        return message;
    }

    /// <summary>
    /// Sets a private field on an object using reflection.
    /// </summary>
    private static void SetPrivateField(object instance, string fieldName, object value)
    {
        var field = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
        if (field == null)
            throw new InvalidOperationException($"Field '{fieldName}' not found on type '{instance.GetType().FullName}'");
        field.SetValue(instance, value);
    }

    /// <summary>
    /// Sets a private property on an object using reflection.
    /// </summary>
    private static void SetPrivateProperty(object instance, string propertyName, object value)
    {
        var prop = instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);
        if (prop == null)
            throw new InvalidOperationException($"Property '{propertyName}' not found on type '{instance.GetType().FullName}'");
        prop.SetValue(instance, value);
    }
}
