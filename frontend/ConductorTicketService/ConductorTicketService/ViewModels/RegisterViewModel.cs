using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class RegisterViewModel : ObservableObject, IRegisterViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    private string _firstName = string.Empty;

    public string FirstName
    {
        get => _firstName;
        set => SetProperty(ref _firstName, value);
    }

    private string _surname = string.Empty;

    public string Surname
    {
        get => _surname;
        set => SetProperty(ref _surname, value);
    }

    private string _email = string.Empty;

    public string Email
    {
        get => _email;
        set => SetProperty(ref _email, value);
    }

    private string _password = string.Empty;

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    private string _confirmPassword = string.Empty;

    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => SetProperty(ref _confirmPassword, value);
    }

    // Simulates the code from the invite email a conductor would normally get.
    private string _registrationCode = string.Empty;

    public string RegistrationCode
    {
        get => _registrationCode;
        set => SetProperty(ref _registrationCode, value);
    }

    private string? _errorMessage;

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (SetProperty(ref _errorMessage, value))
            {
                OnPropertyChanged(nameof(HasErrorMessage));
            }
        }
    }

    public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand RegisterCommand { get; set; }
    public ICommand NavigateToLoginCommand { get; set; }

    public RegisterViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        RegisterCommand = new AsyncRelayCommand(Register);
        NavigateToLoginCommand = new AsyncRelayCommand(_navigationService.NavigateToLoginPageAsync);
    }

    private async Task Register()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(RegistrationCode))
        {
            ErrorMessage = "U moet uw e-mailadres, wachtwoord en registratiecode invullen.";
            return;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "De wachtwoorden komen niet overeen.";
            return;
        }

        IsBusy = true;
        try
        {
            await _authService.RegisterAsync(
                Email.Trim(),
                Password,
                // Names are optional: send null instead of an empty string.
                string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                string.IsNullOrWhiteSpace(Surname) ? null : Surname.Trim(),
                RegistrationCode.Trim());

            // Don't keep the passwords in memory longer than needed.
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            await _navigationService.NavigateToMainPageAsync();
        }
        catch (InvalidRegistrationCodeException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (EmailAlreadyInUseException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
