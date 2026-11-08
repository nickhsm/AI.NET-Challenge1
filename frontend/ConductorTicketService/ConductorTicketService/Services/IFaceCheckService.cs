using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface IFaceCheckService
{
    Task<CheckFaceResult?> CheckFaceAsync(Stream photo);
}