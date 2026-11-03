using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;

namespace TicketService.Infrastructure.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private TicketServiceDbContext _context;

    public PassengerRepository(TicketServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Passenger?> GetPassengerById(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        return passenger;
    }

    public async Task<bool> PassengerExistsAsync(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        return passenger != null;
    }

    public void RegisterFace(int passengerId, Guid azureFaceId)
    {
        var passenger = _context.Passengers.Find(passengerId);

        if (passenger == null)
        {
            throw new InvalidOperationException($"Passenger with id {passengerId} not found.");
        }

        passenger.FacePersonId = azureFaceId;
        _context.SaveChanges();
    }

    public async Task UnregisterFace(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        if (passenger == null)
        {
            throw new InvalidOperationException($"Passenger with id {passengerId} not found.");
        }

        passenger.FacePersonId = null;
        await _context.SaveChangesAsync();
    }
}