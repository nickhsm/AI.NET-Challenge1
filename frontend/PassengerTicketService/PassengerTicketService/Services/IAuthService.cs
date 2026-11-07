namespace PassengerTicketService.Services;

public interface IAuthService
{
    Task LoginAsync(string email, string password);
    Task RegisterAsync(string email, string password, string? firstName, string? surname, DateOnly? dateOfBirth);
    Task<bool> IsLoggedInAsync();
    Task<string?> GetTokenAsync();
    void Logout();
}
