using Microsoft.Extensions.Logging;
using Moq;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.Services;
using Vosaio.AI.Travel.Data;

namespace Vosaio.AI.Travel.API.Tests.Services;

[TestFixture]
public class ItineraryServiceTests
{
    private Mock<IAIIntegrationService> _aiIntegrationServiceMock;
    private Mock<IItineraryRepository> _itineraryRepositoryMock;
    private Mock<ILogger<ItineraryService>> _loggerMock;
    private ItineraryService _itineraryService;

    [SetUp]
    public void Setup()
    {
        _aiIntegrationServiceMock = new Mock<IAIIntegrationService>();
        _itineraryRepositoryMock = new Mock<IItineraryRepository>();
        _loggerMock = new Mock<ILogger<ItineraryService>>();
        _itineraryService = new ItineraryService(
            _aiIntegrationServiceMock.Object,
            _itineraryRepositoryMock.Object,
            _loggerMock.Object
        );
    }

    [Test]
    public async Task GenerateItineraryAsync_ShouldReturnItineraryResponseAndSaveRecord()
    {
        // Arrange: Create a valid travel request.
        var travelRequest = new TravelRequest
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

        // Arrange: Build an expected itinerary response.
        var itineraryDetails = new ItineraryDetails
        {
            Hotels = new List<Hotel>
                {
                    new Hotel { Name = "Hotel A", Rating = 4.5, EstimatedCost = 150 }
                },
            Activities = new List<Activity>
                {
                    new Activity { Name = "Sightseeing", Time = "10:00 AM", EstimatedCost = 50 }
                },
            Restaurants = new List<Restaurant>
                {
                    new Restaurant { Name = "Sushi Bar", Cuisine = "Japanese", EstimatedCost = 100 }
                },
            TotalEstimatedCost = 300
        };

        var expectedResponse = new ItineraryResponse
        {
            Destination = "Tokyo",
            TravelDates = new List<DateTime>
                {
                    travelRequest.TravelDates[0],
                    travelRequest.TravelDates[1]
                },
            Itinerary = itineraryDetails
        };

        // Arrange: Set up the AI integration service to return the expected itinerary.
        _aiIntegrationServiceMock
            .Setup(x => x.GetItineraryFromAIAsync(travelRequest))
            .ReturnsAsync(expectedResponse);

        // Act: Call the GenerateItineraryAsync method.
        var result = await _itineraryService.GenerateItineraryAsync(travelRequest);

        // Assert: Validate the response.
        Assert.NotNull(result);
        Assert.AreEqual(expectedResponse.Destination, result.Destination);
        Assert.AreEqual(expectedResponse.Itinerary.TotalEstimatedCost, result.Itinerary.TotalEstimatedCost);

        // Verify that the repository's AddItineraryRecordAsync was called once with a record matching the request and response.
        _itineraryRepositoryMock.Verify(x => x.AddItineraryRecordAsync(
            It.Is<ItineraryRecord>(record =>
                record.Destination == expectedResponse.Destination &&
                record.StartDate == travelRequest.TravelDates[0] &&
                record.EndDate == travelRequest.TravelDates[1] &&
                record.Budget == travelRequest.Budget &&
                record.Interests.Contains("history") &&
                record.ItineraryJson.Contains("Hotels")
            ),
            It.IsAny<CancellationToken>()),
            Times.Once
        );
    }

    [Test]
    public void GenerateItineraryAsync_WhenAIIntegrationThrows_ShouldLogAndRethrow()
    {
        // Arrange: Create a valid travel request.
        var travelRequest = new TravelRequest
        {
            Destination = "Paris",
            TravelDates = new List<DateTime>
                {
                    new DateTime(2025, 7, 1),
                    new DateTime(2025, 7, 10)
                },
            Budget = 1500,
            Interests = new List<string> { "art", "culture" }
        };

        // Arrange: Setup the AI integration service to throw an exception.
        var exceptionMessage = "Test exception";
        _aiIntegrationServiceMock
            .Setup(x => x.GetItineraryFromAIAsync(travelRequest))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act & Assert: Verify that an exception is thrown.
        var ex = Assert.ThrowsAsync<Exception>(async () => await _itineraryService.GenerateItineraryAsync(travelRequest));
        Assert.That(ex.Message, Does.Contain(exceptionMessage));

        // Verify that the logger's Log method was called at least once with LogLevel.Error.
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error generating itinerary")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }
}