using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IChangePasswordViewModel
{
    string CurrentPassword { get; set; }
    string NewPassword { get; set; }
    string ConfirmNewPassword { get; set; }
    string? ErrorMessage { get; set; }
    bool HasErrorMessage { get; }
    string? SuccessMessage { get; set; }
    bool HasSuccessMessage { get; }
    bool IsBusy { get; set; }
    ICommand ChangePasswordCommand { get; set; }
}
