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

        Task LoadProfileAsync();

        // Acties
        ICommand LogoutCommand { get; }
        ICommand EditAccountCommand { get; }
        ICommand AddFaceIdCommand { get; }

        // Navigatiebalk (vanuit jouw aangeleverde code)
        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
        ICommand NavigateProfileCommand { get; }
    }
}