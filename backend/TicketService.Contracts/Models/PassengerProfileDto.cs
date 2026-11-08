namespace TicketService.Contracts.Models;

public class PassengerProfileDto
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string Email { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    
    // Handig voor de MAUI-app om te weten of het gezicht al geregistreerd is
    public bool HasFaceRegistered { get; set; } 

    public PassengerProfileDto(int id, string? firstName, string? surname, string email, DateOnly? dateOfBirth, bool hasFaceRegistered)
    {
        Id = id;
        FirstName = firstName;
        Surname = surname;
        Email = email;
        DateOfBirth = dateOfBirth;
        HasFaceRegistered = hasFaceRegistered;
    }
}