using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IStartViewModel
    {
        ICommand LoginCommand { get; }
        ICommand NavigateRegisterCommand { get; }
        ICommand NavigateLoginCommand { get; }
    }
}