using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IPassengerRepository
{
    public Task<Passenger?> GetPassengerById(int passengerId);
    public Task<bool> PassengerExistsAsync(int passengerId);
    public void RegisterFace(int passengerId, Guid azureFaceId);
    public Task UnregisterFace(int passengerId);
}