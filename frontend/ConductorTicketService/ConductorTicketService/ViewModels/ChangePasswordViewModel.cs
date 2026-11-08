using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class ChangePasswordViewModel : ObservableObject, IChangePasswordViewModel
{
    private readonly IAccountService _accountService;

    private string _currentPassword = string.Empty;

    public string CurrentPassword
    {
        get => _currentPassword;
        set => SetProperty(ref _currentPassword, value);
    }

    private string _newPassword = string.Empty;

    public string NewPassword
    {
        get => _newPassword;
        set => SetProperty(ref _newPassword, value);
    }

    private string _confirmNewPassword = string.Empty;

    public string ConfirmNewPassword
    {
        get => _confirmNewPassword;
        set => SetProperty(ref _confirmNewPassword, value);
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

    private string? _successMessage;

    public string? SuccessMessage
    {
        get => _successMessage;
        set
        {
            if (SetProperty(ref _successMessage, value))
            {
                OnPropertyChanged(nameof(HasSuccessMessage));
            }
        }
    }

    public bool HasSuccessMessage => !string.IsNullOrEmpty(SuccessMessage);

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }

    public ICommand ChangePasswordCommand { get; set; }

    public ChangePasswordViewModel(IAccountService accountService)
    {
        _accountService = accountService;
        ChangePasswordCommand = new AsyncRelayCommand(ChangePassword);
    }

    private async Task ChangePassword()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            ErrorMessage = "U moet uw huidige en nieuwe wachtwoord invullen.";
            return;
        }

        // Only the app knows about the confirmation, the backend just gets the new password.
        if (NewPassword != ConfirmNewPassword)
        {
            ErrorMessage = "De nieuwe wachtwoorden komen niet overeen.";
            return;
        }

        IsBusy = true;
        try
        {
            await _accountService.ChangePasswordAsync(CurrentPassword, NewPassword);

            // Don't keep the passwords in memory (or on screen) longer than needed.
            CurrentPassword = string.Empty;
            NewPassword = string.Empty;
            ConfirmNewPassword = string.Empty;
            SuccessMessage = "Uw wachtwoord is gewijzigd.";
        }
        catch (WrongPasswordException ex)
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
