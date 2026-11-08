using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface ITicketService
{
    public Task<IEnumerable<TicketListItemResult>> GetTicketsByFacePersonIdAsync(string facePersonId);
    public Task ConsumeTicketAsync(int ticketId);
    public Task<TicketQrCodeResult> GetTicketByQrCodeAsync(Guid qrCode);
}