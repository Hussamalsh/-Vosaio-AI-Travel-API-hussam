using Microsoft.EntityFrameworkCore;

namespace Vosaio.AI.Travel.Data;

public class TravelContext : DbContext
{
    public TravelContext(DbContextOptions<TravelContext> options) : base(options)
    {
    }

    public DbSet<ItineraryRecord> ItineraryRecords { get; set; }
}
