namespace TicketService.Contracts.Models;

// What the conductor app gets back from GET api/conductor/me.
// No password (not even the hash): it never leaves the backend.
public class ConductorProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }

    public ConductorProfileDto(int id, string email, string? firstName, string? surname)
    {
        Id = id;
        Email = email;
        FirstName = firstName;
        Surname = surname;
    }
}
