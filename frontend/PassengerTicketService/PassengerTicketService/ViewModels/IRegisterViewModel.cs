using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IRegisterViewModel
    {
        // Formulier velden
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        string Password { get; set; }
        string ConfirmPassword { get; set; }
        DateTime BirthDate { get; set; }

        // Zichtbaarheid
        bool IsPasswordVisible { get; set; }
        bool IsConfirmPasswordVisible { get; set; }
        bool IsPasswordHidden { get; }
        bool IsConfirmPasswordHidden { get; }

        // Acties
        ICommand RegisterCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand TogglePasswordVisibilityCommand { get; }
        ICommand ToggleConfirmPasswordVisibilityCommand { get; }
    }
}