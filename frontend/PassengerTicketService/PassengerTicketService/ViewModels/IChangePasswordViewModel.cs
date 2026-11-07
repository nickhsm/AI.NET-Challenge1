using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IChangePasswordViewModel
    {
        // Formulier velden
        string CurrentPassword { get; set; }
        string NewPassword { get; set; }
        string ConfirmNewPassword { get; set; }

        // Zichtbaarheid voor de oogjes
        bool IsCurrentPasswordVisible { get; set; }
        bool IsNewPasswordVisible { get; set; }
        bool IsConfirmPasswordVisible { get; set; }

        bool IsCurrentPasswordHidden { get; }
        bool IsNewPasswordHidden { get; }
        bool IsConfirmPasswordHidden { get; }

        // Acties
        ICommand SaveCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand ToggleCurrentPasswordVisibilityCommand { get; }
        ICommand ToggleNewPasswordVisibilityCommand { get; }
        ICommand ToggleConfirmPasswordVisibilityCommand { get; }
    }
}