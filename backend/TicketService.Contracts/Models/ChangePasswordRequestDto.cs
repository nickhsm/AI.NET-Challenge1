using System.ComponentModel.DataAnnotations;

namespace TicketService.Contracts.Models;

public class ChangePasswordRequestDto
{
    [Required]
    public string CurrentPassword { get; set; }

    [Required]
    //[MinLength(6, ErrorMessage = "Het nieuwe wachtwoord moet minimaal 6 tekens bevatten.")]
    public string NewPassword { get; set; }
}