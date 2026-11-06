using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class HomeViewModel : ObservableObject, IHomeViewModel
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string startStation;

        [ObservableProperty]
        private string endStation;

        public ICommand BuyTicketCommand { get; private set; } = null!;
        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;
        public ICommand SwapStationsCommand { get; private set; } = null!;

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            BuyTicketCommand = new AsyncRelayCommand(GoToYourTravelPage);

            // Navbar: FAB (train) to de ticketpagina
            NavigateTicketsCommand = new AsyncRelayCommand(_navigationService.NavigateToTicketsPageAsync);

            NavigateHomeCommand = new RelayCommand(() => { });

            NavigateProfileCommand = new AsyncRelayCommand(_navigationService.NavigateToProfilePageAsync);

            SwapStationsCommand = new RelayCommand(SwapStations);
        }

        // De logica voor het wisselen
        private void SwapStations()
        {
            // Wissel de waarden om via een tijdelijke variabele
            var temp = StartStation;
            StartStation = EndStation;
            EndStation = temp;
        }

        private async Task GoToYourTravelPage()
        {
            if (string.IsNullOrWhiteSpace(StartStation) || string.IsNullOrWhiteSpace(EndStation))
                return;

            // Maak het DTO aan met de standaard waarden
            var request = new TravelRequest
            {
                StartStation = StartStation.ToUpper(),
                EndStation = EndStation.ToUpper(),
                PassengerCount = 1,     // Standaard 1 passagier
                IsRoundTrip = false,    // Standaard enkele reis
                ExpirationDate = DateOnly.FromDateTime(DateTime.Today)
            };

            // Navigeer en geef de request mee via de NavigationService
            await _navigationService.NavigateToYourTravelPageAsync(request);
        }
    }
}