namespace TicketService.Contracts.Models;

public class PriceCalculationRequestDto
{
    public int PassengerCount { get; set; }
    public bool IsRoundTrip { get; set; }
}