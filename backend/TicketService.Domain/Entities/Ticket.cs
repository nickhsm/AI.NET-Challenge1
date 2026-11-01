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
    [ForeignKey("PassengerId")]
    public int PassengerId { get; set; }
    public Passenger? Passenger { get; set; }

    [Required]
    [ForeignKey("StationId")]
    public int StartingStationId { get; set; }
    public Station? StartingStation { get; set; }

    [Required]
    [ForeignKey("StationId")]
    public int DestinationStationId { get; set; }
    public Station? DestinationStation { get; set; }

    public ICollection<StationTicket>  StationTickets { get; set; } = new List<StationTicket>();
    public ICollection<Station> Stations { get; set; } = new List<Station>();

    public Ticket(Guid guidQrCode, DateOnly expirationDate, int passengerId, int startingStationId, int destinationStationId)
    {
        GuidQrCode = guidQrCode;
        ExpirationDate = expirationDate;
        PassengerId = passengerId;
        StartingStationId = startingStationId;
        DestinationStationId = destinationStationId;
    }
}