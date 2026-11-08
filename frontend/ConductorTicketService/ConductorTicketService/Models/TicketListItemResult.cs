namespace ConductorTicketService.Models;

public class TicketListItemResult
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; } = string.Empty;
    public string DestinationStation { get; set; } = string.Empty;
}