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
        modelBuilder.Entity<Ticket>()
            .HasMany(t => t.Stations)
            .WithMany(s => s.Tickets)
            .UsingEntity<StationTicket>();
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.StartingStation)
            .WithMany(s => s.DepartingTickets)
            .HasForeignKey(t => t.StartingStationId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.DestinationStation)
            .WithMany(s => s.ArrivingTickets)
            .HasForeignKey(t => t.DestinationStationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}