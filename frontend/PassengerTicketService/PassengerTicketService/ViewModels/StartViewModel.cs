using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class StartViewModel : ObservableObject, IStartViewModel
    {
        private readonly INavigationService _navigationService;

        public ICommand NavigateRegisterCommand { get; }

        public ICommand NavigateLoginCommand { get; }

        public StartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            
            // Acties voor de Profiel-pagina knoppen (tijdelijk een alert)
            NavigateRegisterCommand = new AsyncRelayCommand(_navigationService.NavigateToRegisterPageAsync);
            NavigateLoginCommand = new AsyncRelayCommand(_navigationService.NavigateToLoginPageAsync);
        }
    }
}