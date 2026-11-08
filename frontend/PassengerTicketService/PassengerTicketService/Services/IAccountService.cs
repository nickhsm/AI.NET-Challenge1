using PassengerTicketService.Models;

namespace PassengerTicketService.Services;

public interface IAccountService
{
    Task<PassengerProfileResult> GetProfileAsync();
    Task UpdateProfileAsync(string email, string? firstName, string? surname, DateOnly? dateOfBirth);
    Task ChangePasswordAsync(string currentPassword, string newPassword);
    Task DeleteAccountAsync();
    Task RegisterFaceAsync(Stream photo);
    Task UnregisterFaceAsync();
}
