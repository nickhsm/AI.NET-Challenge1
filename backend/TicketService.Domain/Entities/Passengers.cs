using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketService.Domain.Entities;

public class Passenger : IUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
    public string? FirstName { get; set; }
    public string? Surname { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    // This is for Azure Face API.
    // The Id here is for out database.
    // The FacePersonId is the Id that Azure gives for a face when the user uploads there picture.
    public Guid? FacePersonId { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public Passenger(string email, string password)
    {
        Email = email;
        Password = password;
    }
}