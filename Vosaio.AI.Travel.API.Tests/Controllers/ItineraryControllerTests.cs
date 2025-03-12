using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Vosaio.AI.Travel.API.Controllers;
using Vosaio.AI.Travel.API.Models;
using Vosaio.AI.Travel.API.Services;

namespace Vosaio.AI.Travel.API.Tests.Controllers;

[TestFixture]
public class ItineraryControllerTests
{
    private Mock<IItineraryService> _mockItineraryService;
    private Mock<ILogger<ItineraryController>> _mockLogger;
    private ItineraryController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockItineraryService = new Mock<IItineraryService>();
        _mockLogger = new Mock<ILogger<ItineraryController>>();

        // Create the controller with mocked dependencies.
        _controller = new ItineraryController( _mockItineraryService.Object, _mockLogger.Object);
    }

    [Test]
    public async Task Generate_ValidRequest_ReturnsOkWithItinerary()
    {
        // Arrange
        var request = new TravelRequest
        {
            Destination = "Paris",
            TravelDates = new() { DateTime.Parse("2025-06-01"), DateTime.Parse("2025-06-07") },
            Budget = 2000,
            Interests = new() { "Museums", "Gastronomy" }
        };

        var expectedResponse = new ItineraryResponse
        {
            Destination = "Paris",
            TravelDates = new List<DateTime> { DateTime.Parse("2025-06-01"), DateTime.Parse("2025-06-07") },
            Itinerary = new ItineraryDetails
            {
                Hotels = new List<Hotel> { new Hotel { Name = "Hotel de Paris", Rating = 4.5, EstimatedCost = 800 } },
                Activities = new List<Activity> { new Activity { Name = "Louvre visit", Time = "Morning", EstimatedCost = 30 } },
                Restaurants = new List<Restaurant> { new Restaurant { Name = "Le Gourmet", Cuisine = "French", EstimatedCost = 50 } },
                TotalEstimatedCost = 1000
            }
        };

        // Mock the service call.
        _mockItineraryService
            .Setup(s => s.GenerateItineraryAsync(It.IsAny<TravelRequest>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _controller.Generate(request);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(actionResult);
        var okResult = actionResult as OkObjectResult;
        Assert.IsNotNull(okResult);

        // Check if the response matches.
        var actualResponse = okResult.Value as ItineraryResponse;
        Assert.IsNotNull(actualResponse);
        Assert.AreEqual(expectedResponse.Destination, actualResponse.Destination);
        Assert.AreEqual(expectedResponse.Itinerary.TotalEstimatedCost, actualResponse.Itinerary.TotalEstimatedCost);

        // Verify the service was called exactly once.
        _mockItineraryService.Verify( s => s.GenerateItineraryAsync(It.IsAny<TravelRequest>()), Times.Once);
    }

    [Test]
    public async Task Generate_InvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new TravelRequest(); // Missing required fields
                                           // Force the ModelState to be invalid
        _controller.ModelState.AddModelError("Destination", "Destination is required");

        // Act
        var actionResult = await _controller.Generate(request);

        // Assert
        Assert.IsInstanceOf<BadRequestObjectResult>(actionResult);
        var badRequestResult = actionResult as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult);
        Assert.IsInstanceOf<SerializableError>(badRequestResult.Value);

        // Verify that the service wasn't called
        _mockItineraryService.Verify(
            s => s.GenerateItineraryAsync(It.IsAny<TravelRequest>()),
            Times.Never
        );
    }

    [Test]
    public async Task Generate_ServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new TravelRequest
        {
            Destination = "London",
            TravelDates = new() { DateTime.Parse("2025-07-10"), DateTime.Parse("2025-07-15") },
            Budget = 1500,
            Interests = new() { "Theater", "History" }
        };

        // Setup mock to throw an exception
        _mockItineraryService
            .Setup(s => s.GenerateItineraryAsync(It.IsAny<TravelRequest>()))
            .ThrowsAsync(new Exception("Test exception"));

        // Act
        var actionResult = await _controller.Generate(request);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(actionResult);
        var objectResult = actionResult as ObjectResult;
        Assert.IsNotNull(objectResult);
        Assert.AreEqual(500, objectResult.StatusCode);

        // Verify the service was called exactly once.
        _mockItineraryService.Verify( s => s.GenerateItineraryAsync(It.IsAny<TravelRequest>()),  Times.Once);
    }
}
