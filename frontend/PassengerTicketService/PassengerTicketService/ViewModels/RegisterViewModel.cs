using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class RegisterViewModel : ObservableObject, IRegisterViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private string firstName = string.Empty;

        [ObservableProperty]
        private string lastName = string.Empty;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private string confirmPassword = string.Empty;

        [ObservableProperty]
        private DateTime birthDate = DateTime.Today.AddYears(-18);

        // Logica voor het oogje (zichtbaarheid)
        [ObservableProperty]
        private bool isPasswordVisible = false;

        [ObservableProperty]
        private bool isConfirmPasswordVisible = false;

        // NIEUW: Deze properties zijn het tegenovergestelde van de bovenstaande
        public bool IsPasswordHidden => !IsPasswordVisible;
        public bool IsConfirmPasswordHidden => !IsConfirmPasswordVisible;

        // NIEUW: Zorg dat de UI update als de zichtbaarheid verandert
        partial void OnIsPasswordVisibleChanged(bool value)
        {
            OnPropertyChanged(nameof(IsPasswordHidden));
        }

        partial void OnIsConfirmPasswordVisibleChanged(bool value)
        {
            OnPropertyChanged(nameof(IsConfirmPasswordHidden));
        }

        public ICommand RegisterCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand TogglePasswordVisibilityCommand { get; private set; } = null!;
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; private set; } = null!;


        public RegisterViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
            BindCommands();
        }

        private void BindCommands()
        {
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
            TogglePasswordVisibilityCommand = new RelayCommand(() => IsPasswordVisible = !IsPasswordVisible);
            ToggleConfirmPasswordVisibilityCommand = new RelayCommand(() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible);
        }

        private async Task RegisterAsync()
        {
            // Basis validatie
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alstublieft alle verplichte velden in.", "OK");
                return;
            }

            // 2. Validatie van het e-mailadres (Regex)
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(Email, emailPattern))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Voer een geldig e-mailadres in (bijv. naam@domein.nl).", "OK");
                return;
            }

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De wachtwoorden komen niet overeen.", "OK");
                return;
            }

            // TODO: Validatie van de geboortedatum (bijv. minimaal 18 jaar oud) kan hier ook worden toegevoegd.

            // 3. Registreren bij de backend. Bij succes slaat de AuthService de token op, dus de gebruiker is meteen ingelogd.
            IsBusy = true;
            try
            {
                await _authService.RegisterAsync(
                    Email.Trim(),
                    Password,
                    FirstName.Trim(),
                    // Achternaam is optioneel: stuur null in plaats van een lege string.
                    string.IsNullOrWhiteSpace(LastName) ? null : LastName.Trim(),
                    // De backend verwacht alleen een datum, zonder tijd.
                    DateOnly.FromDateTime(BirthDate));
            }
            catch (EmailAlreadyInUseException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
                return;
            }
            catch (ApiException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
                return;
            }
            catch (HttpRequestException)
            {
                // The phone can't reach the backend (no wifi, wrong network, API not running).
                await Application.Current.MainPage.DisplayAlert("Fout",
                    "We kunnen de server niet bereiken. Heeft u verbinding met het internet?", "OK");
                return;
            }
            finally
            {
                IsBusy = false;
            }

            // Don't keep the passwords in memory longer than needed.
            Password = string.Empty;
            ConfirmPassword = string.Empty;

            // Navigeer naar de Home-pagina (en reset de stack), net zoals na het inloggen.
            await _navigationService.NavigateToHomeAfterLoginAsync();
        }
    }
}