namespace ConductorTicketService.Models;

public class TicketQrCodeResult
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; } = string.Empty;
    public string DestinationStation { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public bool Used { get; set; }
    public bool IsValidToday { get; set; }

    // Only a ticket that is unused and valid today can be consumed.
    public bool CanConsume => !Used && IsValidToday;
}
