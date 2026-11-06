using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class StartViewModel : ObservableObject, IStartViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand LoginCommand { get; }

        public ICommand NavigateRegisterCommand { get; }

        public ICommand NavigateLoginCommand => throw new NotImplementedException();

        public StartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Acties voor de Profiel-pagina knoppen (tijdelijk een alert)
            LoginCommand = new AsyncRelayCommand(LoginAsync);
            NavigateRegisterCommand = new AsyncRelayCommand(_navigationService.NavigateToRegisterPageAsync);
        }

        private async Task LoginAsync()
        {
            // TODO: Vervang dit later door _navigationService.NavigateToLoginPageAsync();
            await Application.Current.MainPage.DisplayAlert("Aanmelden", "Navigeer naar het inlogscherm", "OK");
        }
    }
}