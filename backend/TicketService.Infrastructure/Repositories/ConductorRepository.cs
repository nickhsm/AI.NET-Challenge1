using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;

namespace TicketService.Infrastructure.Repositories;

public class ConductorRepository : IConductorRepository
{
    private readonly TicketServiceDbContext _context;

    public ConductorRepository(TicketServiceDbContext context)
    {
        _context = context;
    }

    public async Task<Conductor?> GetConductorByEmailAsync(string email)
    {
        return await _context.Conductors.SingleOrDefaultAsync(c => c.Email == email);
    }

    public async Task AddConductorAsync(Conductor conductor)
    {
        _context.Conductors.Add(conductor);
        await _context.SaveChangesAsync();
    }
}