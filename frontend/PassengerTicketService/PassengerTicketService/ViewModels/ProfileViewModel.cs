using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ProfileViewModel : ObservableObject, IProfileViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand NavigateHomeCommand { get; }
        public ICommand NavigateTicketsCommand { get; }

        public ProfileViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Acties voor de Profiel-pagina knoppen (tijdelijk een alert)
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            RegisterCommand = new AsyncRelayCommand(RegisterAsync);
            
            // Navigatiebalk acties
            NavigateHomeCommand = new AsyncRelayCommand(_navigationService.NavigateToHomePageAsync);
            NavigateTicketsCommand = new AsyncRelayCommand(_navigationService.NavigateToTicketsPageAsync);
        }

        private async Task LoginAsync()
        {
            // TODO: Vervang dit later door _navigationService.NavigateToLoginPageAsync();
            await Application.Current.MainPage.DisplayAlert("Aanmelden", "Navigeer naar het inlogscherm", "OK");
        }

        private async Task RegisterAsync()
        {
            // TODO: Vervang dit later door _navigationService.NavigateToRegisterPageAsync();
            await Application.Current.MainPage.DisplayAlert("Account aanmaken", "Navigeer naar het registratiescherm", "OK");
        }
    }
}