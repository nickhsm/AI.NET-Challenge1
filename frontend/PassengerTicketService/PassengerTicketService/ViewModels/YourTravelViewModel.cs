using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

        public DateTime MinimumDate => DateTime.Today;

        public ICommand ConfirmTravelCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;
        public ICommand SelectSingleTripCommand { get; private set; } = null!;
        public ICommand SelectRoundTripCommand { get; private set; } = null!;

        public YourTravelViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            CurrentRequest.ReturnExpirationDate = DateTime.Today;
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
            SelectSingleTripCommand = new RelayCommand(() =>
            {
                CurrentRequest.IsRoundTrip = false;
            });
            SelectRoundTripCommand = new RelayCommand(() => CurrentRequest.IsRoundTrip = true);
        }

        private async Task GoToShoppingCart()
        {
            if (CurrentRequest == null) return;

            if (CurrentRequest.ExpirationDate < DateTime.Today)
                CurrentRequest.ExpirationDate = DateTime.Today;

            if (CurrentRequest.IsRoundTrip && CurrentRequest.ReturnExpirationDate < CurrentRequest.ExpirationDate)
                CurrentRequest.ReturnExpirationDate = CurrentRequest.ExpirationDate;

            // 2. Navigeer naar de winkelwagen en geef de UPDATED request mee
            await _navigationService.NavigateToShoppingCartPageAsync(CurrentRequest);
        }
    }
}