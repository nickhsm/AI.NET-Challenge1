using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.DbContexts;

public class TicketServiceDbContext(DbContextOptions<TicketServiceDbContext> options) : DbContext(options)
{
    public DbSet<Station> Stations { get; set; }
    public DbSet<Passenger> Passengers { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<StationTicket> StationTickets { get; set; }
    public DbSet<Conductor> Conductors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}