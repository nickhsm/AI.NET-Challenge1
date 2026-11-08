using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class AccountViewModel : ObservableObject, IAccountViewModel
{
    private readonly IAccountService _accountService;

    // OnAppearing also fires when coming back from another page (like change password).
    // Loading again then would overwrite what the conductor already typed.
    private bool _isProfileLoaded;

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

    public ICommand SaveCommand { get; set; }
    public ICommand ChangePasswordCommand { get; set; }

    public AccountViewModel(IAccountService accountService, INavigationService navigationService)
    {
        _accountService = accountService;
        SaveCommand = new AsyncRelayCommand(Save);
        ChangePasswordCommand = new AsyncRelayCommand(navigationService.NavigateToChangePasswordPageAsync);
    }

    public async Task LoadProfileAsync()
    {
        if (_isProfileLoaded)
        {
            return;
        }

        ErrorMessage = null;
        IsBusy = true;
        try
        {
            var profile = await _accountService.GetProfileAsync();

            // The names can be null in the database, the entries need a string.
            FirstName = profile.FirstName ?? string.Empty;
            Surname = profile.Surname ?? string.Empty;
            Email = profile.Email;
            _isProfileLoaded = true;
        }
        catch (ApiException ex)
        {
            ErrorMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
            Debug.WriteLine(ex);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            ErrorMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task Save()
    {
        ErrorMessage = null;
        SuccessMessage = null;

        // Saving before the profile is loaded would send empty fields and wipe the names.
        if (!_isProfileLoaded)
        {
            ErrorMessage = "Uw gegevens zijn nog niet geladen.";
            return;
        }

        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = "U moet uw e-mailadres invullen.";
            return;
        }

        IsBusy = true;
        try
        {
            await _accountService.UpdateProfileAsync(
                Email.Trim(),
                // Same as register: an empty name is sent as null.
                string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                string.IsNullOrWhiteSpace(Surname) ? null : Surname.Trim());

            SuccessMessage = "Uw gegevens zijn opgeslagen.";
        }
        catch (EmailAlreadyInUseException)
        {
            ErrorMessage = "Er bestaat al een account met dit e-mailadres.";
        }
        catch (ApiException ex)
        {
            ErrorMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
            Debug.WriteLine(ex);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            ErrorMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
            Debug.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
