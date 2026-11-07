using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class LoginViewModel : ObservableObject, ILoginViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isPasswordVisible = false;

        // Het tegenovergestelde van IsPasswordVisible (zodat we geen converter nodig hebben)
        public bool IsPasswordHidden => !IsPasswordVisible;

        public ICommand LoginCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand TogglePasswordVisibilityCommand { get; private set; } = null!;  

        public LoginViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
            BindCommands();
        }

        private void BindCommands()
        {
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
            TogglePasswordVisibilityCommand = new RelayCommand(() => IsPasswordVisible = !IsPasswordVisible);
        }

        // Update de UI als het oogje wordt geklikt
        partial void OnIsPasswordVisibleChanged(bool value)
        {
            OnPropertyChanged(nameof(IsPasswordHidden));
        }

        private async Task LoginAsync()
        {
            // 1. Check op lege velden
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Vul alstublieft uw e-mailadres en wachtwoord in.", "OK");
                return;
            }

            // 2. Validatie van het e-mailadres (Regex)
            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            if (!Regex.IsMatch(Email, emailPattern))
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Voer een geldig e-mailadres in.", "OK");
                return;
            }

            // 3. Inloggen bij de backend. Bij succes slaat de AuthService de token op in SecureStorage.
            IsBusy = true;
            try
            {
                await _authService.LoginAsync(Email.Trim(), Password);
            }
            catch (InvalidLoginException ex)
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

            // Don't keep the password in memory longer than needed.
            Password = string.Empty;

            // Navigeer naar de Home-pagina (en reset de stack)
            await _navigationService.NavigateToHomeAfterLoginAsync();
        }
    }
}