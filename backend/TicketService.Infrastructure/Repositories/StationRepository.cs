using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;

namespace TicketService.Infrastructure.Repositories;

public class StationRepository : IStationRepository
{
    private readonly TicketServiceDbContext _context;

    public StationRepository(TicketServiceDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Station>> GetStationsAsync(string? searchQuery)
    {
        var query = _context.Stations.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            // Gebruik ILike voor case-insensitive zoeken in PostgreSQL
            var searchTerm = $"%{searchQuery.Trim()}%";
            query = query.Where(s => EF.Functions.ILike(s.Name, searchTerm));
        }

        return await query
            .OrderBy(s => s.Name)
            .Take(20)
            .ToListAsync();
    }
}