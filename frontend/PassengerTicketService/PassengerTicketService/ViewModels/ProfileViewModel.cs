using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IProfileViewModel
    {
        private readonly INavigationService _navigationService;

        // Voor nu hardcoded. Later haal je dit uit je ingelogde user object of API.
        public string UserName => "JONATHAN YANG";

        public ICommand LogoutCommand { get; private set; } = null!;
        public ICommand EditAccountCommand { get; private set; } = null!;
        public ICommand AddFaceIdCommand { get; private set; } = null!;

        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;

        public ProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            // Functionaliteit voor de lijst items
            EditAccountCommand = new AsyncRelayCommand(_navigationService.NavigateToEditProfilePageAsync);
            AddFaceIdCommand = new AsyncRelayCommand(AddFaceIdAsync);

            // Logout logica
            LogoutCommand = new AsyncRelayCommand(LogoutAsync);

            // Navigatiebalk acties
            NavigateHomeCommand = new AsyncRelayCommand(_navigationService.NavigateToHomePageAsync);
            NavigateTicketsCommand = new AsyncRelayCommand(_navigationService.NavigateToTicketsPageAsync);
            NavigateHomeCommand = new RelayCommand(() => { });

            // We zijn al op de Profile pagina, dus dit is een leeg commando
            NavigateProfileCommand = new RelayCommand(() => { });
        }

        private async Task AddFaceIdAsync()
        {
            // TODO: Roep later de biometric API aan (bijv. via Plugin.Fingerprint)
            await Application.Current.MainPage.DisplayAlert("Beveiliging", "Gezichtsherkenning toevoegen...", "OK");
        }

        private async Task LogoutAsync()
        {
            var confirm = await Application.Current.MainPage.DisplayAlert("Uitloggen", "Weet je zeker dat je wilt uitloggen?", "Ja", "Nee");
            if (confirm)
            {
                // Verwijder hier eventueel je JWT token uit SecureStorage:
                // SecureStorage.Default.Remove("auth_token");

                // Stuur de gebruiker terug naar de inlog/startpagina
                // (Zorg dat je deze methode in je NavigationService hebt toegevoegd)
                await _navigationService.NavigateToStartPageAsync();
            }
        }
    }
}