using Microsoft.EntityFrameworkCore;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.DbContexts;
using TicketService.Infrastructure.Exceptions;

namespace TicketService.Infrastructure.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly TicketServiceDbContext _context;

    public TicketRepository(TicketServiceDbContext context)
    {
        _context = context;
    }

    private DateOnly Today()
    {
        return DateOnly.FromDateTime(DateTime.Now);
    }

    public async Task<bool> PassengerHasValidTicketAsync(Guid facePersonId)
    {
        DateOnly today = Today();
        return await _context.Tickets.AnyAsync(t =>
            // Ignore possible null error. Compiler does not know that I have required on that field in Ticker
            t.Passenger!.FacePersonId == facePersonId
            && t.ExpirationDate == today
            && t.Used == false);
    }

    public async Task<IEnumerable<Ticket>> GetValidTicketsAsync(Guid facePersonId)
    {
        DateOnly today = Today();

        return await _context.Tickets.Include(t => t.Passenger)
            .Include(t => t.StartingStation)
            .Include(t => t.DestinationStation)
            .Where(t => t.Passenger!.FacePersonId == facePersonId
                        && t.ExpirationDate == today
                        && t.Used == false)
            .ToListAsync();
    }

    public async Task ConsumeTicketAsync(int ticketId)
    {
        // already used?
        Ticket? ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket is null)
        {
            throw new TicketNotFoundException();
        }

        if (ticket.Used)
        {
            throw new TicketAlreadyUsedException();
        }

        if (ticket.ExpirationDate != Today())
        {
            throw new TicketNotValidTodayException();
        }

        ticket.Used = true;
        await _context.SaveChangesAsync();
    }

    public Task<bool> TicketExistsAsync(int ticketId)
    {
        return _context.Tickets.AnyAsync(t => t.Id == ticketId);
    }

    public async Task<Ticket?> GetTicketByQrCodeAsync(Guid qrCode)
    {
        var ticket = await _context.Tickets
            .Include(t => t.Passenger)
            .Include(t => t.StartingStation)
            .Include(t => t.DestinationStation)
            .SingleOrDefaultAsync(t => t.GuidQrCode == qrCode);

        return ticket;
    }
}