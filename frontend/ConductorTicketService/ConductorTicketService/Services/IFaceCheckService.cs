using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface IFaceCheckService
{
    public Task<CheckFaceResult?> CheckFaceAsync(Stream photo);
}