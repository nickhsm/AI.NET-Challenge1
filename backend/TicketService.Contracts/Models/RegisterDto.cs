namespace TicketService.Contracts.Models;

public class RegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public DateOnly? DateOfBirth { get; set; }

    public RegisterDto(string email, string password, string? firstName, string? surname, DateOnly? dateOfBirth)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        Surname = surname;
        DateOfBirth = dateOfBirth;
    }
}
