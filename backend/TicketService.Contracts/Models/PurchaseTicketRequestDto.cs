namespace TicketService.Contracts.Models;

public class PurchaseTicketRequestDto
{
    public int PassengerId { get; set; }
    
    public int StartStationId { get; set; }
    public int EndStationId { get; set; }
    
    public int PassengerCount { get; set; }
    public bool IsRoundTrip { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public DateOnly ReturnExpirationDate { get; set; }
}