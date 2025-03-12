using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.OpenAI;

namespace Vosaio.AI.Travel.API.Tests;

[TestFixture]
public class PromptBuilderTests
{
    private IPromptBuilder _promptBuilder;

    [SetUp]
    public void Setup()
    {
        _promptBuilder = new PromptBuilder();
    }

    [Test]
    public void BuildPrompt_NullRequest_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => _promptBuilder.BuildPrompt(null));
    }

    [Test]
    public void BuildPrompt_InvalidTravelDates_ThrowsArgumentException()
    {
        // Case 1: TravelDates is null
        var request1 = new TravelRequest
        {
            Destination = "Tokyo",
            TravelDates = null,
            Budget = 2000,
            Interests = new List<string> { "history", "food", "adventure" }
        };
        Assert.Throws<ArgumentException>(() => _promptBuilder.BuildPrompt(request1));

        // Case 2: TravelDates contains only one date
        var request2 = new TravelRequest
        {
            Destination = "Tokyo",
            TravelDates = new List<DateTime> { DateTime.Now },
            Budget = 2000,
            Interests = new List<string> { "history", "food", "adventure" }
        };
        Assert.Throws<ArgumentException>(() => _promptBuilder.BuildPrompt(request2));
    }

    [Test]
    public void BuildPrompt_ValidRequest_ReturnsExpectedPrompt()
    {
        // Arrange
        var startDate = new DateTime(2025, 6, 1);
        var endDate = new DateTime(2025, 6, 10);
        var request = new TravelRequest
        {
            Destination = "Tokyo",
            TravelDates = new List<DateTime> { startDate, endDate },
            Budget = 2000,
            Interests = new List<string> { "history", "food", "adventure" }
        };

        // Act
        string prompt = _promptBuilder.BuildPrompt(request);

        // Assert: Verify the prompt contains the expected parts.
        Assert.IsNotNull(prompt);
        StringAssert.Contains("Generate a travel itinerary for Tokyo", prompt);
        StringAssert.Contains(startDate.ToString("yyyy-MM-dd"), prompt);
        StringAssert.Contains(endDate.ToString("yyyy-MM-dd"), prompt);
        StringAssert.Contains("2000", prompt);
        StringAssert.Contains("history, food, adventure", prompt);
    }
}
