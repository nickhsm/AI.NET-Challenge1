using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface ILoginViewModel
{
    string Email { get; set; }
    string Password { get; set; }
    string? ErrorMessage { get; set; }
    bool HasErrorMessage { get; }
    bool IsBusy { get; set; }
    ICommand LoginCommand { get; set; }
    ICommand NavigateToRegisterCommand { get; set; }
    Task CheckAlreadyLoggedInAsync();
}