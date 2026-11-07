using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IStationRepository
{
    Task<IEnumerable<Station>> GetStationsAsync(string? searchQuery);
}