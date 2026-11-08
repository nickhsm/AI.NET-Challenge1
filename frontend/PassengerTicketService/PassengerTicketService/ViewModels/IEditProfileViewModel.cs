using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IEditProfileViewModel
    {
        // Formulier velden
        string FirstName { get; set; }
        string LastName { get; set; }
        string Email { get; set; }
        DateTime BirthDate { get; set; }

        // True terwijl we wachten op de backend (spinner)
        bool IsBusy { get; set; }

        string? ErrorMessage { get; set; }
        bool HasErrorMessage { get; }

        // Acties
        ICommand SaveCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand ChangePasswordCommand { get; }
        ICommand DeleteAccountCommand { get; }

        Task LoadProfileAsync();
    }
}