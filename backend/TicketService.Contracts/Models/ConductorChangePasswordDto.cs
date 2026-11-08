namespace TicketService.Contracts.Models;

// Body of PATCH api/conductor/me/password.
// CurrentPassword is checked first, so someone who picks up an unlocked phone can't lock the conductor out.
public class ConductorChangePasswordDto
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }

    public ConductorChangePasswordDto(string currentPassword, string newPassword)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }
}
