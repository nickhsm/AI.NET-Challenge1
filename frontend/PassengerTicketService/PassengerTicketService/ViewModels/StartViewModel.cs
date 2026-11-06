using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class StartViewModel : ObservableObject, IStartViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand NavigateRegisterCommand { get; private set; } = null!;

        public ICommand NavigateLoginCommand { get; private set; } = null!;

        public StartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            // Acties voor de Profiel-pagina knoppen (tijdelijk een alert)
            NavigateRegisterCommand = new AsyncRelayCommand(_navigationService.NavigateToRegisterPageAsync);
            NavigateLoginCommand = new AsyncRelayCommand(_navigationService.NavigateToLoginPageAsync);
        }
    }
}