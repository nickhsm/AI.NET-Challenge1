using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface ILoginViewModel
    {
        // Formulier velden
        string Email { get; set; }
        string Password { get; set; }

        // True terwijl we wachten op de backend (spinner)
        bool IsBusy { get; set; }

        // Voor het oogje
        bool IsPasswordVisible { get; set; }
        bool IsPasswordHidden { get; }

        // Acties
        ICommand LoginCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand TogglePasswordVisibilityCommand { get; }
    }
}