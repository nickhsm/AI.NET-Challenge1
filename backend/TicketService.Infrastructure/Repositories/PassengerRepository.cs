using Microsoft.EntityFrameworkCore;
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

    public async Task<Passenger?> GetPassengerByIdAsync(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        return passenger;
    }

    public async Task<Passenger?> GetPassengerByFacePersonIdAsync(Guid facePersonId)
    {
        Passenger? passenger = await _context.Passengers.SingleOrDefaultAsync(p => p.FacePersonId == facePersonId);

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

    public async Task UnregisterFaceAsync(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        if (passenger == null)
        {
            throw new InvalidOperationException($"Passenger with id {passengerId} not found.");
        }

        passenger.FacePersonId = null;
        await _context.SaveChangesAsync();
    }

    public async Task UnregisterAllFacesAsync()
    {
        // ExecuteUpdateAsync runs one UPDATE query directly in the database.
        // It doesn't load the passengers and doesn't need SaveChangesAsync.
        await _context.Passengers
            .Where(p => p.FacePersonId != null)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.FacePersonId, (Guid?)null));
    }

    public async Task<Passenger?> GetPassengerByEmailAsync(string email)
    {
        return await _context.Passengers.SingleOrDefaultAsync(p => p.Email == email);
    }

    public async Task AddPassengerAsync(Passenger passenger)
    {
        _context.Passengers.Add(passenger);
        await _context.SaveChangesAsync();
    }
}