using System.ComponentModel.DataAnnotations;

namespace TicketService.Contracts.Models;

public class PriceCalculationRequestDto
{
    [Range(1, 100)]
    public int PassengerCount { get; set; }
    public bool IsRoundTrip { get; set; }
}
