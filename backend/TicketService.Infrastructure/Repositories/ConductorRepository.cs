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

    public async Task<Conductor?> GetConductorByIdAsync(int id)
    {
        return await _context.Conductors.FindAsync(id);
    }

    // Returns false when the conductor doesn't exist, so the controller can answer 404.
    public async Task<bool> UpdateConductorProfileAsync(int id, string email, string? firstName, string? surname)
    {
        var conductor = await _context.Conductors.FindAsync(id);

        if (conductor == null)
        {
            return false;
        }

        conductor.Email = email;
        conductor.FirstName = firstName;
        conductor.Surname = surname;

        await _context.SaveChangesAsync();
        return true;
    }

    // Takes an already hashed password: hashing is done by IPasswordHasher in the API layer.
    public async Task<bool> UpdateConductorPasswordAsync(int id, string hashedPassword)
    {
        var conductor = await _context.Conductors.FindAsync(id);

        if (conductor == null)
        {
            return false;
        }

        conductor.Password = hashedPassword;

        await _context.SaveChangesAsync();
        return true;
    }
}