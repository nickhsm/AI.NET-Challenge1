using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketService.Domain.Entities;

public class Ticket
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    // Guid is .NET's naming for uuid
    [Required]
    public Guid GuidQrCode { get; set; }
    [Required]
    public DateOnly ExpirationDate { get; set; }
    [Required]
    public bool Used { get; set; }

    [Required]
    [ForeignKey("Passenger")]
    public int PassengerId { get; set; }
    public Passenger? Passenger { get; set; }

    [Required]
    [ForeignKey("StartingStation")]
    public int StartingStationId { get; set; }
    public Station? StartingStation { get; set; }

    [Required]
    [ForeignKey("DestinationStation")]
    public int DestinationStationId { get; set; }
    public Station? DestinationStation { get; set; }

    public ICollection<StationTicket>  StationTickets { get; set; } = new List<StationTicket>();
    public ICollection<Station> Stations { get; set; } = new List<Station>();

    public Ticket(DateOnly expirationDate, int passengerId, int startingStationId, int destinationStationId)
    {
        GuidQrCode = Guid.NewGuid();
        ExpirationDate = expirationDate;
        PassengerId = passengerId;
        StartingStationId = startingStationId;
        DestinationStationId = destinationStationId;
        Used = false;
    }
}