namespace PassengerTicketService.Models;

// Same shape as PriceCalculationResponseDto in the backend, so ReadFromJsonAsync can read it.
public class PriceResult
{
    public decimal OutboundPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
