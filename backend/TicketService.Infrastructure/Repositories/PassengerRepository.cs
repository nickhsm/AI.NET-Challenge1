using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;
using TicketService.Infrastructure.Exceptions; 
using TicketService.Infrastructure.Services;  


namespace TicketService.Infrastructure.Repositories;

public class PassengerRepository : IPassengerRepository
{
    private TicketServiceDbContext _context;
    private readonly IFaceRecognitionService _faceRecognitionService; 

    public PassengerRepository(TicketServiceDbContext context, IFaceRecognitionService faceRecognitionService)
    {
        _context = context;
        _faceRecognitionService = faceRecognitionService;
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
    
    public async Task<bool> DeletePassengerAsync(int passengerId)
    {
        var passenger = await _context.Passengers.FindAsync(passengerId);

        if (passenger == null)
        {
            return false;
        }

        // 1. Verwijder eerst het gezicht uit Azure (indien er een gezicht is geregistreerd)
        if (passenger.FacePersonId.HasValue)
        {
            try
            {
                await _faceRecognitionService.DeleteAsync(passenger.FacePersonId.Value);
            }
            catch (AzureServiceException)
            {
                // Als Azure faalt, gooien we de error door.
                // We verwijderen het account dan NIET, zodat de data consistent blijft.
                // De controller vangt deze exception op.
                throw; 
            }
        }

        // 2. Pas als Azure succesvol is (of als er geen gezicht was), verwijderen we de passagier
        _context.Passengers.Remove(passenger);
        await _context.SaveChangesAsync();

        return true;
    }
    
    public async Task UpdateProfileAsync(int passengerId, string firstName, string surname, string email, DateOnly? dateOfBirth)
    {
        // 1. Haal de data op uit de databank (zoals RegisterFace doet)
        var passenger = await _context.Passengers.FindAsync(passengerId);

        if (passenger == null)
        {
            throw new InvalidOperationException($"Passenger with id {passengerId} not found.");
        }

        // 2. Pas de gegevens aan
        passenger.FirstName = firstName;
        passenger.Surname = surname;
        passenger.Email = email.Trim().ToLower();
        passenger.DateOfBirth = dateOfBirth;

        // 3. Sla de wijzigingen op
        await _context.SaveChangesAsync();
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        // We gebruiken ToLower() zodat de check niet uitmaakt of er hoofdletters of kleine letters worden gebruikt
        return await _context.Passengers.AnyAsync(p => p.Email.ToLower() == email.ToLower());
    }
}