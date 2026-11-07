using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IRegisterViewModel
{
    string FirstName { get; set; }
    string Surname { get; set; }
    string Email { get; set; }
    string Password { get; set; }
    string ConfirmPassword { get; set; }
    string RegistrationCode { get; set; }
    string? ErrorMessage { get; set; }
    bool HasErrorMessage { get; }
    bool IsBusy { get; set; }
    ICommand RegisterCommand { get; set; }
    ICommand NavigateToLoginCommand { get; set; }
}
