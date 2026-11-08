using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IProfileViewModel
    {
        string UserName { get; }

        // True terwijl we wachten op de backend (spinner)
        bool IsBusy { get; set; }

        string? ErrorMessage { get; set; }
        bool HasErrorMessage { get; }

        // "toevoegen" or "verwijderen", depending on whether a face is registered.
        string FaceActionText { get; }

        Task LoadProfileAsync();

        // Acties
        ICommand LogoutCommand { get; }
        ICommand EditAccountCommand { get; }
        ICommand ToggleFaceCommand { get; }

        // Navigatiebalk (vanuit jouw aangeleverde code)
        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
        ICommand NavigateProfileCommand { get; }
    }
}