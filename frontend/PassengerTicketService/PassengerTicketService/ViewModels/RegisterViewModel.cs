using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class RegisterViewModel : ObservableObject, IRegisterViewModel
    {
        private readonly INavigationService _navigationService;

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
        private DateOnly birthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)); // Standaard 18 jaar geleden

        // Logica voor het oogje (zichtbaarheid)
        [ObservableProperty]
        private bool isPasswordVisible = false;

        [ObservableProperty]
        private bool isConfirmPasswordVisible = false;

        public ICommand RegisterCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }


        public RegisterViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

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

            if (Password != ConfirmPassword)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "De wachtwoorden komen niet overeen.", "OK");
                return;
            }

            // TODO: Hier komt later de API call (bijv. _apiService.RegisterAsync(...))

            // Tijdelijke simulatie van een API call
            await Application.Current.MainPage.DisplayAlert("Succes", "Je account is aangemaakt! Je kunt nu inloggen.", "OK");

            // Ga terug naar home pagina na succesvolle registratie   
            await _navigationService.NavigateBackAsync();
        }
    }
}