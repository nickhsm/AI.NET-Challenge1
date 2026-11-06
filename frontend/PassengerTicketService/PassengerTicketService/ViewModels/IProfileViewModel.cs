using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IProfileViewModel
    {
        string UserName { get; }

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