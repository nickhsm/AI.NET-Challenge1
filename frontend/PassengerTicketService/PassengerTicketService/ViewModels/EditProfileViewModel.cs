using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class EditProfileViewModel : ObservableObject, IEditProfileViewModel
    {
        private const string ServerErrorMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
        private const string NoConnectionMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";

        private readonly INavigationService _navigationService;
        private readonly IAccountService _accountService;
        private readonly IAuthService _authService;

        // OnAppearing also fires when coming back from another page (like change password).
        // Loading again then would overwrite what the passenger already typed.
        private bool _isProfileLoaded;

        // The date shown in the picker when the passenger has no birth date.
        // Still this exact date when saving means no date was chosen, so we send null.
        private DateTime? _noBirthDatePlaceholder;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        // Starts on a valid date: the picker can't show the DateTime default (year 1).
        [ObservableProperty]
        private DateTime birthDate = DateTime.Today;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand SaveCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand ChangePasswordCommand { get; private set; } = null!;
        public ICommand DeleteAccountCommand { get; private set; } = null!;

        public EditProfileViewModel(INavigationService navigationService, IAccountService accountService,
            IAuthService authService)
        {
            _navigationService = navigationService;
            _accountService = accountService;
            _authService = authService;

            BindCommands();
        }

        private void BindCommands()
        {
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
            ChangePasswordCommand = new AsyncRelayCommand(_navigationService.NavigateToChangePassword);
            DeleteAccountCommand = new AsyncRelayCommand(DeleteAccountAsync);
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
                LastName = profile.Surname ?? string.Empty;
                Email = profile.Email;

                if (profile.DateOfBirth is DateOnly dateOfBirth)
                {
                    BirthDate = dateOfBirth.ToDateTime(TimeOnly.MinValue);
                }
                else
                {
                    _noBirthDatePlaceholder = BirthDate;
                }

                _isProfileLoaded = true;
            }
            catch (ApiException ex)
            {
                ErrorMessage = ServerErrorMessage;
                Debug.WriteLine(ex);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                ErrorMessage = NoConnectionMessage;
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy)
            {
                return;
            }

            ErrorMessage = null;

            // Saving before the profile is loaded would overwrite the real data with empty fields.
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

            DateOnly? dateOfBirth = BirthDate == _noBirthDatePlaceholder
                ? null
                : DateOnly.FromDateTime(BirthDate);

            IsBusy = true;
            try
            {
                await _accountService.UpdateProfileAsync(
                    Email.Trim(),
                    // Same as register: an empty name is sent as null.
                    string.IsNullOrWhiteSpace(FirstName) ? null : FirstName.Trim(),
                    string.IsNullOrWhiteSpace(LastName) ? null : LastName.Trim(),
                    dateOfBirth);
            }
            catch (EmailAlreadyInUseException ex)
            {
                ErrorMessage = ex.Message;
                return;
            }
            catch (ApiException ex)
            {
                ErrorMessage = ServerErrorMessage;
                Debug.WriteLine(ex);
                return;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                ErrorMessage = NoConnectionMessage;
                Debug.WriteLine(ex);
                return;
            }
            finally
            {
                IsBusy = false;
            }

            // Only reached when the save worked: every catch above returns.
            await _navigationService.NavigateBackAsync();
        }

        private async Task DeleteAccountAsync()
        {
            if (IsBusy)
            {
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Account verwijderen",
                "Weet je zeker dat je je account permanent wilt verwijderen? Dit kan niet ongedaan worden gemaakt.",
                "Ja, verwijder",
                "Annuleren");

            if (!confirm)
            {
                return;
            }

            ErrorMessage = null;
            IsBusy = true;
            try
            {
                await _accountService.DeleteAccountAsync();
            }
            catch (ApiException ex)
            {
                // For example a 502: the account was NOT deleted, so the passenger stays logged in here.
                ErrorMessage = "Je account kon niet verwijderd worden. Probeer het later opnieuw.";
                Debug.WriteLine(ex);
                return;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                ErrorMessage = NoConnectionMessage;
                Debug.WriteLine(ex);
                return;
            }
            finally
            {
                IsBusy = false;
            }

            // The token belongs to an account that no longer exists.
            _authService.Logout();

            await Application.Current.MainPage.DisplayAlert("Verwijderd", "Je account is verwijderd.", "OK");
            await _navigationService.NavigateToStartPageAsync();
        }
    }
}
