namespace ConductorTicketService.Models;

public class CheckFaceResult
{
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public string FacePersonId { get; set; } = string.Empty;
    public bool HasTickets { get; set; }
}