using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;

namespace TicketService.Infrastructure.Seeders;

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly TicketServiceDbContext _context;
    private readonly IPasswordHasher<IUser> _hasher;

    public DatabaseSeeder(TicketServiceDbContext context, IPasswordHasher<IUser> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    public async Task<bool> SeedAsync()
    {
        // Only seed an empty database.
        // Wiping existing data would also wipe FacePersonIds, while the Persons stay in Azure.
        if (await _context.Passengers.AnyAsync() || await _context.Stations.AnyAsync())
        {
            return false;
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);

        // Stations
        var antwerpen = new Station("Antwerpen-Centraal");
        var brussel = new Station("Brussel-Centraal");
        var gent = new Station("Gent-Sint-Pieters");
        var leuven = new Station("Leuven");
        var brugge = new Station("Brugge");

        _context.Stations.AddRange(antwerpen, brussel, gent, leuven, brugge);

        // Conductor
        var conductor = new Conductor("conductor@test.be", "password")
        {
            FirstName = "Jan",
            Surname = "Peeters",
            DateOfBirth = new DateOnly(1985, 3, 12)
        };
        _context.Conductors.Add(conductor);

        // Passengers (no FacePersonId, faces are registered through the API)
        var nick = new Passenger("nick@test.be", "password")
        {
            FirstName = "Nick",
            Surname = "Test",
            DateOfBirth = new DateOnly(2003, 5, 20)
        };
        var sarah = new Passenger("sarah@test.be", "password")
        {
            FirstName = "Sarah",
            Surname = "Janssens",
            DateOfBirth = new DateOnly(2001, 9, 2)
        };
        var tom = new Passenger("tom@test.be", "password")
        {
            FirstName = "Tom",
            Surname = "Maes",
            DateOfBirth = new DateOnly(1998, 1, 15)
        };
        _context.Passengers.AddRange(nick, sarah, tom);

        // Zorgt dat elke wachtwoord die we boven plaintext hebben ingevuld gehashed wordt.
        foreach (IUser user in new IUser[] { conductor, sarah, tom, nick })
        {
            user.Password = _hasher.HashPassword(user, user.Password);
        }

        // Tickets
        // The ids passed to the constructor are 0 because the stations and passengers don't have ids yet.
        // EF Core fills in the foreign keys from the navigation properties when saving.
        _context.Tickets.AddRange(
            // Nick: one valid ticket, one used, one from yesterday and one for tomorrow
            CreateTicket(nick, antwerpen, brussel, today),
            CreateTicket(nick, brussel, antwerpen, today, used: true),
            CreateTicket(nick, antwerpen, gent, today.AddDays(-1)),
            CreateTicket(nick, gent, brugge, today.AddDays(1)),
            // Sarah: one valid ticket
            CreateTicket(sarah, leuven, brussel, today)
            // Tom: no tickets
        );

        await _context.SaveChangesAsync();

        return true;
    }

    private static Ticket CreateTicket(Passenger passenger, Station from, Station to, DateOnly date, bool used = false)
    {
        return new Ticket(date, 0, 0, 0)
        {
            Passenger = passenger,
            StartingStation = from,
            DestinationStation = to,
            Used = used
        };
    }
}
