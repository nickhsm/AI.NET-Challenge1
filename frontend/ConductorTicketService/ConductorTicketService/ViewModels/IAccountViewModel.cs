using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IAccountViewModel
{
    string FirstName { get; set; }
    string Surname { get; set; }
    string Email { get; set; }
    string? ErrorMessage { get; set; }
    bool HasErrorMessage { get; }
    string? SuccessMessage { get; set; }
    bool HasSuccessMessage { get; }
    bool IsBusy { get; set; }
    ICommand SaveCommand { get; set; }
    ICommand ChangePasswordCommand { get; set; }

    Task LoadProfileAsync();
}
