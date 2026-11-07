using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class LoginViewModel : ObservableObject, ILoginViewModel
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

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

    public ICommand LoginCommand { get; set; }

    public LoginViewModel(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
        LoginCommand = new AsyncRelayCommand(Login);
    }

    public async Task CheckAlreadyLoggedInAsync()
    {
        if (await _authService.IsLoggedInAsync())
        {
            await _navigationService.NavigateToMainPageAsync();
        }
    }

    private async Task Login()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "U moet uw e-mail en wachtwoord invullen.";
            return;
        }

        IsBusy = true;
        try
        {
            await _authService.LoginAsync(Email.Trim(), Password);

            // Don't keep the password in memory longer than needed.
            Password = string.Empty;
            await _navigationService.NavigateToMainPageAsync();
        }
        catch (InvalidLoginException ex)
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