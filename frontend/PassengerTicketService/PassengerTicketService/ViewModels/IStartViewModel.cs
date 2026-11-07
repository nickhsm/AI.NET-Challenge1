using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IStartViewModel
    {
        ICommand NavigateRegisterCommand { get; }
        ICommand NavigateLoginCommand { get; }

        // Gaat meteen naar Home als er nog een geldige token is opgeslagen
        Task CheckAlreadyLoggedInAsync();
    }
}