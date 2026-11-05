using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IProfileViewModel
    {
        ICommand LoginCommand { get; }
        ICommand RegisterCommand { get; }
        
        // Navbar commands
        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
    }
}