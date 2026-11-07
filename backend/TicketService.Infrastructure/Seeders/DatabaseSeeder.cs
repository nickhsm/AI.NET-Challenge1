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
            // The QR codes are fixed (instead of random) so they can be used in tests.
            // The last digit is the ticket id after a reset.
            // Nick: one valid ticket, one used, one from yesterday and one for tomorrow
            CreateTicket(nick, antwerpen, brussel, today, "00000000-0000-0000-0000-000000000001", 2.60m),
            CreateTicket(nick, brussel, antwerpen, today, "00000000-0000-0000-0000-000000000002", 2.60m, used: true),
            CreateTicket(nick, antwerpen, gent, today.AddDays(-1), "00000000-0000-0000-0000-000000000003", 2.60m),
            CreateTicket(nick, gent, brugge, today.AddDays(1), "00000000-0000-0000-0000-000000000004", 2.60m),
            // Sarah: one valid ticket
            CreateTicket(sarah, leuven, brussel, today, "00000000-0000-0000-0000-000000000005", 5.20m)
            // Tom: no tickets
        );

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task ResetAsync()
    {
        // TRUNCATE empties the tables in one query.
        // RESTART IDENTITY makes the ids start at 1 again, so the seeded tickets always get the same ids.
        // CASCADE also empties tables that point to these tables with a foreign key.
        await _context.Database.ExecuteSqlRawAsync(
            "TRUNCATE \"Tickets\", \"StationTickets\", \"Passengers\", \"Conductors\", \"Stations\" RESTART IDENTITY CASCADE");

        // Forget entities EF Core already loaded, they no longer exist in the database.
        _context.ChangeTracker.Clear();

        await SeedAsync();
    }

    private static Ticket CreateTicket(Passenger passenger, Station from, Station to, DateOnly date, string qrCode,
        decimal price, bool used = false)
    {
        return new Ticket(date, 0, 0, 0, price)
        {
            GuidQrCode = Guid.Parse(qrCode),
            Passenger = passenger,
            StartingStation = from,
            DestinationStation = to,
            Used = used
        };
    }
}
