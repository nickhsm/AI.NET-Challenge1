using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IConductorRepository
{
    public Task<Conductor?> GetConductorByEmailAsync(string email);
    public Task AddConductorAsync(Conductor conductor);
}