namespace ConductorTicketService.Services;

public interface IAuthService
{
    public Task LoginAsync(string email, string password);
    public Task RegisterAsync(string email, string password, string? firstName, string? surname, string registrationCode);
    public Task<bool> IsLoggedInAsync();
    public Task<string?> GetTokenAsync();
    public void Logout();
}