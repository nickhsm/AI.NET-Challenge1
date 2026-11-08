using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public interface IAccountService
{
    public Task<ConductorProfileResult> GetProfileAsync();
    public Task UpdateProfileAsync(string email, string? firstName, string? surname);
    public Task ChangePasswordAsync(string currentPassword, string newPassword);
}
