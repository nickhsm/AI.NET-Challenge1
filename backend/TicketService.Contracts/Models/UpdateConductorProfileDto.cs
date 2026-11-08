namespace TicketService.Contracts.Models;

// Body of PUT api/conductor/me. No Id: the backend takes the id from the token,
// so a conductor can only ever change their own account.
public class UpdateConductorProfileDto
{
    public string Email { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }

    public UpdateConductorProfileDto(string email, string? firstName, string? surname)
    {
        Email = email;
        FirstName = firstName;
        Surname = surname;
    }
}
