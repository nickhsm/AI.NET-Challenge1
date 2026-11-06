using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IStartViewModel
    {
        ICommand NavigateRegisterCommand { get; }
        ICommand NavigateLoginCommand { get; }
    }
}