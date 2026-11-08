using System.ComponentModel.DataAnnotations;

namespace TicketService.Contracts.Models;

public class PurchaseTicketRequestDto
{
    public int StartStationId { get; set; }
    public int EndStationId { get; set; }
    
    [Range(1, 100)]
    public int PassengerCount { get; set; }
    public bool IsRoundTrip { get; set; }
    public DateOnly ExpirationDate { get; set; }
    public DateOnly ReturnExpirationDate { get; set; }
}