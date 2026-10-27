using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketService.Domain.Entities;

public class Station
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    
    public ICollection<StationTicket> StationTickets { get; set; } = new List<StationTicket>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> DepartingTickets { get; set; } = new List<Ticket>();
    public ICollection<Ticket> ArrivingTickets { get; set; } = new List<Ticket>();
    
    public Station(string name)
    {
        Name = name;
    }
}