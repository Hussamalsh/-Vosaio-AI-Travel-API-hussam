namespace Vosaio.AI.Travel.Data;

public interface IItineraryRepository
{
    Task AddItineraryRecordAsync(ItineraryRecord record, CancellationToken cancellationToken = default);
    Task<IEnumerable<ItineraryRecord>> GetItineraryRecordsAsync(CancellationToken cancellationToken = default);
}