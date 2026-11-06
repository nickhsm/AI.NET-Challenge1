namespace TicketService.Contracts.Models;

public class CheckFaceDto
{
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string FacePersonId { get; set; }
    public bool HasTickets { get; set; }

    public CheckFaceDto(string? firstName, string? surname, string facePersonId, bool hasTickets)
    {
        FirstName = firstName;
        Surname = surname;
        FacePersonId = facePersonId;
        HasTickets = hasTickets;
    }
}