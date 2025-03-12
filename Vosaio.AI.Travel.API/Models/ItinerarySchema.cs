namespace Vosaio.AI.Travel.API.Models;

/// <summary>
/// Provides the JSON schema used for validating the AI-generated itinerary.
/// </summary>
public static class ItinerarySchema
{
    /// <summary>
    /// The full JSON schema as a string.  
    /// Make sure this matches the model you expect in <see cref="ItineraryResponse"/>.
    /// </summary>
    public const string Value = @"
{
  ""type"": ""object"",
  ""properties"": {
    ""Destination"": { ""type"": ""string"" },
    ""TravelDates"": {
      ""type"": ""array"",
      ""items"": { ""type"": ""string"" }
    },
    ""Itinerary"": {
      ""type"": ""object"",
      ""properties"": {
        ""Hotels"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""Name"": { ""type"": ""string"" },
              ""Rating"": { ""type"": ""number"" },
              ""EstimatedCost"": { ""type"": ""number"" }
            },
            ""required"": [ ""Name"", ""Rating"", ""EstimatedCost"" ],
            ""additionalProperties"": false
          }
        },
        ""Activities"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""Name"": { ""type"": ""string"" },
              ""Time"": { ""type"": ""string"" },
              ""EstimatedCost"": { ""type"": ""number"" }
            },
            ""required"": [ ""Name"", ""Time"", ""EstimatedCost"" ],
            ""additionalProperties"": false
          }
        },
        ""Restaurants"": {
          ""type"": ""array"",
          ""items"": {
            ""type"": ""object"",
            ""properties"": {
              ""Name"": { ""type"": ""string"" },
              ""Cuisine"": { ""type"": ""string"" },
              ""EstimatedCost"": { ""type"": ""number"" }
            },
            ""required"": [ ""Name"", ""Cuisine"", ""EstimatedCost"" ],
            ""additionalProperties"": false
          }
        },
        ""TotalEstimatedCost"": { ""type"": ""number"" }
      },
      ""required"": [ ""Hotels"", ""Activities"", ""Restaurants"", ""TotalEstimatedCost"" ],
      ""additionalProperties"": false
    }
  },
  ""required"": [ ""Destination"", ""TravelDates"", ""Itinerary"" ],
  ""additionalProperties"": false
}";
}
