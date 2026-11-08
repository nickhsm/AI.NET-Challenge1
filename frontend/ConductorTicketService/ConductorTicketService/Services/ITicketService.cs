using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface ITicketService
{
    Task<IEnumerable<TicketListItemResult>> GetTicketsByFacePersonIdAsync(string facePersonId);
    Task ConsumeTicketAsync(int ticketId);
    Task<TicketQrCodeResult> GetTicketByQrCodeAsync(Guid qrCode);
}