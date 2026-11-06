using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IPassengerRepository
{
    public Task<Passenger?> GetPassengerByIdAsync(int passengerId);
    public Task<Passenger?> GetPassengerByFacePersonIdAsync(Guid facePersonId);
    public Task<bool> PassengerExistsAsync(int passengerId);
    public void RegisterFace(int passengerId, Guid azureFaceId);
    public Task UnregisterFaceAsync(int passengerId);
}