using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PassengerTicketService.Models
{
    // Groepeert 1 of 2 tickets voor de UI-kaart
    public class PurchaseDisplayModel
    {
        public string Title => Tickets.Count > 1 ? "Heen- en terugreis" : "Enkele reis";
        public DateOnly ExpirationDate { get; set; }
        public DateTime PurchaseDate { get; set; }
        public List<TicketDisplayModel> Tickets { get; set; } = new();

        // Voor eventuele grouping of styling
        public bool IsRoundTrip => Tickets.Count > 1;
    }

    // Representeert 1 ticket (heen of terug)
    public class TicketDisplayModel
    {
        public Guid GuidQrCode { get; set; }
        public string StationFrom { get; set; } = string.Empty;
        public string StationTo { get; set; } = string.Empty;
        public string JourneyType { get; set; } = string.Empty; // "Heenreis", "Terugreis", "Enkele reis"
        public DateOnly TravelDate { get; set; }
    }
}