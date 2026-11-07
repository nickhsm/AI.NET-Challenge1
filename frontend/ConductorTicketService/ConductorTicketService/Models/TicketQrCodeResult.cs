namespace ConductorTicketService.Models;

public class TicketQrCodeResult
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; }
    public string DestinationStation { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public bool Used { get; set; }
    public bool IsValidToday { get; set; }

    // Only a ticket that is unused and valid today can be consumed.
    public bool CanConsume => !Used && IsValidToday;
}
