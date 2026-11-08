using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IPassengerRepository
{
    public Task<Passenger?> GetPassengerByIdAsync(int passengerId);
    public Task<Passenger?> GetPassengerByFacePersonIdAsync(Guid facePersonId);
    public Task<bool> PassengerExistsAsync(int passengerId);
    public void RegisterFace(int passengerId, Guid azureFaceId);
    public Task UnregisterFaceAsync(int passengerId);
    // Only for development. Removes the FacePersonId from every passenger.
    public Task UnregisterAllFacesAsync();
    public Task<Passenger?> GetPassengerByEmailAsync(string email);
    public Task AddPassengerAsync(Passenger passenger);
    public Task<bool> DeletePassengerAsync(int passengerId);
    public Task<UpdateProfileResult> UpdateProfileAsync(int passengerId, string? firstName, string? surname,
        string email, DateOnly? dateOfBirth);
    public Task<bool> EmailExistsAsync(string email);
}