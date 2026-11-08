namespace PassengerTicketService.Models;

// Matches PassengerProfileDto in the backend (GET api/passenger/me).
public class PassengerProfileResult
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateOnly? DateOfBirth { get; set; }
    public bool HasFaceRegistered { get; set; }
}
