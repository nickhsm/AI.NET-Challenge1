using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;
using TicketService.Infrastructure.Repositories;

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
            var searchTerm = searchQuery.Trim().ToLower();
            query = query
                .Where(s => s.Name.ToLower().Contains(searchTerm))
                .OrderBy(s => !s.Name.ToLower().StartsWith(searchTerm))
                .ThenBy(s => s.Name);
        }
        else
        {
            query = query.OrderBy(s => s.Name);
        }

        return await query
            .Take(20)
            .ToListAsync();
    }
}