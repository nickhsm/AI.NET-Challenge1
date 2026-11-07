using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class StartViewModel : ObservableObject, IStartViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;

        public ICommand NavigateRegisterCommand { get; private set; } = null!;

        public ICommand NavigateLoginCommand { get; private set; } = null!;

        public StartViewModel(INavigationService navigationService, IAuthService authService)
        {
            _navigationService = navigationService;
            _authService = authService;
            BindCommands();
        }

        // Called when the start page appears (also when the app starts).
        // A valid saved token means the user is still logged in, so skip the start page.
        public async Task CheckAlreadyLoggedInAsync()
        {
            if (await _authService.IsLoggedInAsync())
            {
                await _navigationService.NavigateToHomePageAsync();
            }
        }

        private void BindCommands()
        {
            // Acties voor de Profiel-pagina knoppen (tijdelijk een alert)
            NavigateRegisterCommand = new AsyncRelayCommand(_navigationService.NavigateToRegisterPageAsync);
            NavigateLoginCommand = new AsyncRelayCommand(_navigationService.NavigateToLoginPageAsync);
        }
    }
}