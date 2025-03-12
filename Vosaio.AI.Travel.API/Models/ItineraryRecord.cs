namespace Vosaio.AI.Travel.Data;

public class ItineraryRecord
{
    public int Id { get; set; }
    public string Destination { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Budget { get; set; }
    public string Interests { get; set; }
    public string ItineraryJson { get; set; }
    public DateTime CreatedAt { get; set; }
}
