using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketService.Domain.Entities;

public class StationTicket
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StationTicketId { get; set; }
    
    [Required]
    [ForeignKey("StationId")]
    public int StationId { get; set; }
    public Station? Station { get; set; }
    
    [Required]
    [ForeignKey("TicketId")]
    public int TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public StationTicket(int stationId, int ticketId)
    {
        StationId = stationId;
        TicketId = ticketId;
    }
}