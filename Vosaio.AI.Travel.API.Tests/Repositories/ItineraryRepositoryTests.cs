using Microsoft.EntityFrameworkCore;
using Vosaio.AI.Travel.Data;

namespace Vosaio.AI.Travel.API.Tests.Repositories;

[TestFixture]
public class ItineraryRepositoryTests
{
    private TravelContext _context;
    private ItineraryRepository _repository;

    [SetUp]
    public void Setup()
    {
        // Create unique in-memory database options for isolation.
        var options = new DbContextOptionsBuilder<TravelContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TravelContext(options);
        _repository = new ItineraryRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    [Test]
    public async Task AddItineraryRecordAsync_ShouldAddRecord()
    {
        // Arrange: Provide dummy values for required properties.
        var record = new ItineraryRecord
        {
            Destination = "Test Destination",
            CreatedAt = DateTime.UtcNow,
            Interests = "[\"Test Interest\"]",  // Provide as a JSON string or comma-separated list.
            ItineraryJson = "{\"Hotels\":[], \"Activities\":[], \"Restaurants\":[], \"TotalEstimatedCost\":0}"
        };

        // Act
        await _repository.AddItineraryRecordAsync(record);
        var records = await _repository.GetItineraryRecordsAsync();

        // Assert
        Assert.That(records.Count(), Is.EqualTo(1));
        var retrievedRecord = records.First();
        Assert.That(retrievedRecord.Destination, Is.EqualTo("Test Destination"));
    }

    [Test]
    public void AddItineraryRecordAsync_NullRecord_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.ThrowsAsync<ArgumentNullException>(async () => await _repository.AddItineraryRecordAsync(null));
    }

    [Test]
    public async Task GetItineraryRecordsAsync_ShouldReturnEmptyList_WhenNoRecordsExist()
    {
        // Act
        var records = await _repository.GetItineraryRecordsAsync();

        // Assert
        Assert.That(records, Is.Empty);
    }
}