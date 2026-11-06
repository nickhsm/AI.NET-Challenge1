using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class LoginViewModel : ObservableObject, ILoginViewModel
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private string password = string.Empty;

        [ObservableProperty]
        private bool isPasswordVisible = false;

        // Het tegenovergestelde van IsPasswordVisible (zodat we geen converter nodig hebben)
        public bool IsPasswordHidden => !IsPasswordVisible;

        public ICommand LoginCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

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

            // TODO: Hier komt later de API call (bijv. _apiService.LoginAsync(...))
            // Bij een echte implementatie zou je hier de API aanroepen om de gebruiker te authenticeren.
            // Bij succes zou je de JWT token opslaan in SecureStorage.

            // Tijdelijke simulatie van een succesvolle login
            await Application.Current.MainPage.DisplayAlert("Succes", "Je bent succesvol ingelogd!", "OK");

            // Navigeer naar de Home-pagina (en reset de stack)
            await _navigationService.NavigateToHomePageAsync();
        }
    }
}