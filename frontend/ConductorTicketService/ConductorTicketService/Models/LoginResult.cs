namespace ConductorTicketService.Models;

public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}