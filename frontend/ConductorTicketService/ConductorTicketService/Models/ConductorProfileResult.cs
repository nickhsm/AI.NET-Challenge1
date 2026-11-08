namespace ConductorTicketService.Models;

// Matches ConductorProfileDto in the backend (GET api/conductor/me).
public class ConductorProfileResult
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
}
