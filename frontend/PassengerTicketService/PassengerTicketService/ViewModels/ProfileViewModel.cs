using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Services;
using System.Diagnostics;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IProfileViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        private readonly IAccountService _accountService;

        [ObservableProperty]
        private string userName = string.Empty;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LogoutCommand { get; private set; } = null!;
        public ICommand EditAccountCommand { get; private set; } = null!;
        public ICommand AddFaceIdCommand { get; private set; } = null!;

        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;

        public ProfileViewModel(INavigationService navigationService, IAuthService authService,
            IAccountService accountService)
        {
            _navigationService = navigationService;
            _authService = authService;
            _accountService = accountService;
            BindCommands();
        }

        // Called on every OnAppearing, so edits show up when coming back from the edit page.
        public async Task LoadProfileAsync()
        {
            ErrorMessage = null;
            IsBusy = true;
            try
            {
                var profile = await _accountService.GetProfileAsync();

                // Either name can be null: only show the ones that are filled in.
                string fullName = string.Join(" ",
                    new[] { profile.FirstName, profile.Surname }.Where(name => !string.IsNullOrWhiteSpace(name)));

                UserName = fullName.Length > 0 ? fullName : profile.Email;
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