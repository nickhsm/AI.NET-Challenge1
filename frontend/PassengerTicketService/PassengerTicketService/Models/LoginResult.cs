namespace PassengerTicketService.Models;

// Same shape as LoginResponseDto in the backend, so ReadFromJsonAsync can read it.
public class LoginResult
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
