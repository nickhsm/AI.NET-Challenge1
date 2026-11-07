namespace PassengerTicketService.Services;

public interface IAuthService
{
    Task LoginAsync(string email, string password);
    Task<bool> IsLoggedInAsync();
    Task<string?> GetTokenAsync();
    void Logout();
}
