using System.ComponentModel.DataAnnotations;

namespace TicketService.Contracts.Models;

public class UpdateProfileRequestDto : IValidatableObject
{
    private static readonly DateOnly MinimumDateOfBirth = new(1900, 1, 1);

    private string _email = string.Empty;

    [MaxLength(50)]
    public string? FirstName { get; set; }

    [MaxLength(50)]
    public string? Surname { get; set; }

    // Trimmed in the setter, so [EmailAddress] validates the same value that gets stored.
    [Required]
    [EmailAddress]
    [MaxLength(254)]
    public string Email
    {
        get => _email;
        set => _email = value?.Trim() ?? string.Empty;
    }

    public DateOnly? DateOfBirth { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (DateOfBirth is null)
        {
            yield break;
        }

        if (DateOfBirth < MinimumDateOfBirth || DateOfBirth > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            yield return new ValidationResult("Date of birth is not valid.", [nameof(DateOfBirth)]);
        }
    }
}
