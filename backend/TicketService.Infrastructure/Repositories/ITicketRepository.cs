using TicketService.Domain.Entities;

namespace TicketService.Infrastructure.Repositories;

public interface ITicketRepository
{
    public Task<bool> PassengerHasValidTicketAsync(Guid facePersonId);
    // These are tickets that are valid for today
    public Task<IEnumerable<Ticket>> GetValidTicketsAsync(Guid facePersonId);
    public Task ConsumeTicketAsync(int ticketId);
    public Task<bool> TicketExistsAsync(int ticketId);
    public Task<Ticket?> GetTicketByQrCodeAsync(Guid qrCode);
}