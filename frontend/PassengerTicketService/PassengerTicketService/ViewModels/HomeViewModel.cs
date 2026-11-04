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

        public ICommand BuyTicketCommand { get; set; }

        public HomeViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            BuyTicketCommand = new AsyncRelayCommand(GoToYourTravelPage);
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
                IsRoundTrip = false     // Standaard enkele reis
            };

            // Navigeer en geef de request mee via de NavigationService
            await _navigationService.NavigateToYourTravelPageAsync(request);
        }
    }
}