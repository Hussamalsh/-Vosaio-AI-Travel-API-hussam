namespace Vosaio.AI.Travel.API.Models;

public class ItineraryResponse
{
    public string Destination { get; set; }
    public List<DateTime> TravelDates { get; set; }
    public ItineraryDetails Itinerary { get; set; }
}

public class ItineraryDetails
{
    public List<Hotel> Hotels { get; set; } = new List<Hotel>();
    public List<Activity> Activities { get; set; } = new List<Activity>();
    public List<Restaurant> Restaurants { get; set; } = new List<Restaurant>();
    public decimal TotalEstimatedCost { get; set; }
}

public class Hotel
{
    public string Name { get; set; }
    public double Rating { get; set; }
    public decimal EstimatedCost { get; set; }
}

public class Activity
{
    public string Name { get; set; }
    public string Time { get; set; }
    public decimal EstimatedCost { get; set; }
}

public class Restaurant
{
    public string Name { get; set; }
    public string Cuisine { get; set; }
    public decimal EstimatedCost { get; set; }
}
