namespace TicketService.Contracts.Models;

public class ConductorRegisterDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    // Simulates the code that would normally come in the invite email.
    public string RegistrationCode { get; set; }

    public ConductorRegisterDto(string email, string password, string? firstName, string? surname, DateOnly? dateOfBirth, string registrationCode)
    {
        Email = email;
        Password = password;
        FirstName = firstName;
        Surname = surname;
        DateOfBirth = dateOfBirth;
        RegistrationCode = registrationCode;
    }
}
