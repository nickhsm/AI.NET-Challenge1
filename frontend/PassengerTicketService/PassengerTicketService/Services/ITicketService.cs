using PassengerTicketService.Models;

namespace PassengerTicketService.Services;

public interface ITicketService
{
    Task<PriceResult> CalculatePriceAsync(int passengerCount, bool isRoundTrip);
    Task<List<PassengerTicketResult>> GetMyTicketsAsync();
    Task PurchaseTicketsAsync(TravelRequest request);

}
