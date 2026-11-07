using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IProfileViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;

        // Voor nu hardcoded. Later haal je dit uit je ingelogde user object of API.
        public string UserName => "JONATHAN YANG";

        public ICommand LogoutCommand { get; private set; } = null!;
        public ICommand EditAccountCommand { get; private set; } = null!;
        public ICommand AddFaceIdCommand { get; private set; } = null!;

        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;

        public ProfileViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
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
                // Verwijder de token en de vervaldatum uit SecureStorage.
                // Zonder token stuurt de app geen Authorization header meer mee.
                _authService.Logout();

                // Terug naar de startpagina. "//" vervangt de hele stack,
                // dus met "terug" kom je niet meer op een pagina waarvoor je ingelogd moet zijn.
                await _navigationService.NavigateToStartPageAsync();
            }
        }
    }
}