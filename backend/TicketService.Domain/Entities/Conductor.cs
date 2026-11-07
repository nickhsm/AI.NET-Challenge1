using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TicketService.Domain.Entities;

[Index(nameof(Email), IsUnique = true)]
public class Conductor : IUser
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

    public Conductor(string email, string password)
    {
        Email = email;
        Password = password;
    }
}