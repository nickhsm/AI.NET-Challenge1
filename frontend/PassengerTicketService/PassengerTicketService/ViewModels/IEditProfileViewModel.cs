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

        // Acties
        ICommand SaveCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand ChangePasswordCommand { get; }
        ICommand DeleteAccountCommand { get; }
    }
}