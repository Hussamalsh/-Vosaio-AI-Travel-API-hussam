using System.ComponentModel.DataAnnotations;

namespace Vosaio.AI.Travel.API.Models;

public class TravelRequest : IValidatableObject
{
    [Required(ErrorMessage = "Destination is required.")]
    public string Destination { get; set; }

    [Required(ErrorMessage = "Travel dates are required.")]
    public List<DateTime> TravelDates { get; set; } = new List<DateTime>();

    [Required(ErrorMessage = "Budget is required.")]
    [Range(1, double.MaxValue, ErrorMessage = "Budget must be greater than zero.")]
    public decimal Budget { get; set; }

    [Required(ErrorMessage = "At least one interest is required.")]
    public List<string> Interests { get; set; } = new List<string>();

    /// <summary>
    /// Custom validation to ensure exactly two dates are provided 
    /// and the start date is earlier than the end date.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (TravelDates == null || TravelDates.Count != 2)
        {
            yield return new ValidationResult(
                "TravelDates must contain exactly two dates: a start date and an end date.",
                new[] { nameof(TravelDates) });
        }
        else if (TravelDates[0] >= TravelDates[1])
        {
            yield return new ValidationResult(
                "The start date must be earlier than the end date.",
                new[] { nameof(TravelDates) });
        }
    }
}
