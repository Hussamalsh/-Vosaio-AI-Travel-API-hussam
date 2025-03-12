using Microsoft.EntityFrameworkCore;

namespace Vosaio.AI.Travel.Data;

/// <summary>
/// Provides data access for itinerary records using Entity Framework Core.
/// </summary>
public class ItineraryRepository : IItineraryRepository
{
    private readonly TravelContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItineraryRepository"/> class.
    /// </summary>
    /// <param name="context">The EF Core database context.</param>
    /// <exception cref="ArgumentNullException">Thrown if the context is null.</exception>
    public ItineraryRepository(TravelContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new itinerary record to the database.
    /// </summary>
    /// <param name="record">The itinerary record to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the record is null.</exception>
    public async Task AddItineraryRecordAsync(ItineraryRecord record, CancellationToken cancellationToken = default)
    {
        if (record is null)
            throw new ArgumentNullException(nameof(record));

        await _context.ItineraryRecords.AddAsync(record, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all itinerary records from the database.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>An enumerable collection of itinerary records.</returns>
    public async Task<IEnumerable<ItineraryRecord>> GetItineraryRecordsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ItineraryRecords.AsNoTracking().ToListAsync(cancellationToken);
    }
}
