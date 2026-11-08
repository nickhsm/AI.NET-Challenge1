namespace PassengerTicketService.Models;

// Matches PassengerTicketDto in the backend (GET api/ticket/passenger/me/all).
public class PassengerTicketResult
{
    public int Id { get; set; }
    public Guid GuidQrCode { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; } = string.Empty;
    public string DestinationStation { get; set; } = string.Empty;
    public bool Used { get; set; }
    public decimal Price { get; set; }
}
