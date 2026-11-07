using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ShoppingCartViewModel : ObservableObject, IShoppingCartViewModel, IQueryAttributable
    {
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private TravelRequest currentRequest = new();

        // Datum weergave (bijv. "Vandaag, 05/11/2026" of alleen de datum)
        public string ExpirationDateDisplay => $"{CurrentRequest.ExpirationDate:dd/MM/yyyy}";
        public string ReturnExpirationDateDisplay => $"{CurrentRequest.ReturnExpirationDate:dd/MM/yyyy}";

        // Aantal passagiers weergave
        public string PassengerCountDisplay => $"Aantal passagiers: {CurrentRequest.PassengerCount}";

        // Prijsberekening (Stopgap voor API)
        private decimal BasePrice => 2.85m; // Basisprijs per persoon per enkele reis
        private decimal CalculateOutboundPrice() => BasePrice * CurrentRequest.PassengerCount;

        public string OutboundPriceDisplay => $"€ {CalculateOutboundPrice():F2}".Replace(".", ",");
        public string ReturnPriceDisplay => OutboundPriceDisplay; // Retour iszelfde prijs
        public string TotalPriceDisplay => CurrentRequest.IsRoundTrip
            ? $"€ {(CalculateOutboundPrice() * 2):F2}".Replace(".", ",")
            : OutboundPriceDisplay;

        public ICommand PayCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;

        public ShoppingCartViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            BindCommands();
        }

        private void BindCommands()
        {
            PayCommand = new AsyncRelayCommand(PayAsync);
            CancelCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
            NavigateBackCommand = new AsyncRelayCommand(_navigationService.NavigateBackAsync);
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("TravelRequest", out var requestObj) && requestObj is TravelRequest request)
            {
                CurrentRequest = request;
                // Update alle berekende properties
                OnPropertyChanged(nameof(ExpirationDateDisplay));
                OnPropertyChanged(nameof(ReturnExpirationDateDisplay));
                OnPropertyChanged(nameof(PassengerCountDisplay));
                OnPropertyChanged(nameof(OutboundPriceDisplay));
                OnPropertyChanged(nameof(ReturnPriceDisplay));
                OnPropertyChanged(nameof(TotalPriceDisplay));
            }
        }

        private async Task PayAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Betaling", "Betaling succesvol ontvangen!", "OK");
        }
    }
}