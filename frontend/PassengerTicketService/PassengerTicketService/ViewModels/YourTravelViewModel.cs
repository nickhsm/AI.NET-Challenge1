using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PassengerTicketService.Messages;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    // We implementeren IQueryAttributable om de data van de vorige pagina op te vangen
    public partial class YourTravelViewModel : ObservableObject, IYourTravelViewModel, IQueryAttributable
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private TravelRequest currentRequest = new TravelRequest();

        public ICommand ConfirmTravelCommand { get; set; }
        public ICommand NavigateBackCommand { get; set; }

        public YourTravelViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        // Deze methode vangt de data op die NavigationService meestuurt
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("TravelRequest", out var requestObj) && requestObj is TravelRequest request)
            {
                CurrentRequest = request;
            }
        }

        private void BindCommands()
        {
            ConfirmTravelCommand = new AsyncRelayCommand(GoToShoppingCart);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        }

        private async Task GoToShoppingCart()
        {
            if (CurrentRequest == null) return;

            // 1. Stuur een bericht naar de NavBar dat er items in de winkelwagen zitten
            WeakReferenceMessenger.Default.Send(new CartUpdatedMessage(CurrentRequest.PassengerCount));

            // 2. Navigeer naar de winkelwagen en geef de UPDATED request mee
            await _navigationService.NavigateToShoppingCartPageAsync(CurrentRequest);
        }
    }
}