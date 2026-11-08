using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface IConductorRepository
{
    public Task<Conductor?> GetConductorByEmailAsync(string email);
    public Task AddConductorAsync(Conductor conductor);
    public Task<Conductor?> GetConductorByIdAsync(int id);
    public Task<bool> UpdateConductorProfileAsync(int id, string email, string? firstName, string? surname);
    public Task<bool> UpdateConductorPasswordAsync(int id, string hashedPassword);
}