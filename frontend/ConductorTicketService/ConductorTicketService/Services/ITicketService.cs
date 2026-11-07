using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface ITicketService
{
    public Task<IEnumerable<TicketListItemResult>> GetTicketsByFacePersonId(string facePersonId);
}