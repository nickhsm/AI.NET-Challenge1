using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface IAccountService
{
    Task<ConductorProfileResult> GetProfileAsync();
    Task UpdateProfileAsync(string email, string? firstName, string? surname);
    Task ChangePasswordAsync(string currentPassword, string newPassword);
}
