using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketService.Domain.Entities;

[Index(nameof(Email), IsUnique = true)]
[Index(nameof(FacePersonId), IsUnique = true)]
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
    // The Id here is for our database.
    // The FacePersonId is the Id that Azure gives for a face when the user uploads their picture.
    public Guid? FacePersonId { get; set; }
    
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public Passenger(string email, string password)
    {
        Email = email;
        Password = password;
    }
}