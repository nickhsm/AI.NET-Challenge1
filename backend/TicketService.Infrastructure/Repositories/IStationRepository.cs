using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IStationRepository
{
    public Task<IEnumerable<Station>> GetStationsAsync(string? searchQuery);
    public Task<Station?> GetStationByIdAsync(int stationId);
}