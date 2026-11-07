namespace ConductorTicketService.Services;

public interface IAuthService
{
    public Task LoginAsync(string email, string password);
    public Task<bool> IsLoggedInAsync();
    public Task<string?> GetTokenAsync();
    public void Logout();
}