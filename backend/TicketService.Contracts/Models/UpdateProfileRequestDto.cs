using System.ComponentModel.DataAnnotations;

namespace TicketService.Contracts.Models;

public class UpdateProfileRequestDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(50)]
    public string Surname { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public DateOnly DateOfBirth { get; set; }
}