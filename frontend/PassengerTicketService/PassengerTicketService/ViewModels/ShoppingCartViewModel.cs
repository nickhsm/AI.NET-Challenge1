using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class ShoppingCartViewModel : ObservableObject, IShoppingCartViewModel, IQueryAttributable
    {
        private readonly INavigationService _navigationService;
        private readonly ITicketService _ticketService;

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        private bool isBusy = false;

        // Null until the backend has answered.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(OutboundPriceDisplay))]
        [NotifyPropertyChangedFor(nameof(ReturnPriceDisplay))]
        [NotifyPropertyChangedFor(nameof(TotalPriceDisplay))]
        [NotifyPropertyChangedFor(nameof(HasPrice))]
        private PriceResult? price;

        [ObservableProperty]
        private TravelRequest currentRequest = new();

        // Datum weergave (bijv. "Vandaag, 05/11/2026" of alleen de datum)
        public string ExpirationDateDisplay => $"{CurrentRequest.ExpirationDate:dd/MM/yyyy}";
        public string ReturnExpirationDateDisplay => $"{CurrentRequest.ReturnExpirationDate:dd/MM/yyyy}";

        // Aantal passagiers weergave
        public string PassengerCountDisplay => $"Aantal passagiers: {CurrentRequest.PassengerCount}";

        // Paying is only possible once the backend has given a price.
        public bool HasPrice => Price is not null;

        public string OutboundPriceDisplay => FormatPrice(Price?.OutboundPrice);
        public string ReturnPriceDisplay => OutboundPriceDisplay; // Retour iszelfde prijs
        public string TotalPriceDisplay => FormatPrice(Price?.TotalPrice);

        public ICommand PayCommand { get; private set; } = null!;
        public ICommand CancelCommand { get; private set; } = null!;
        public ICommand NavigateBackCommand { get; private set; } = null!;

        public ShoppingCartViewModel(INavigationService navigationService, ITicketService ticketService)
        {
            _navigationService = navigationService;
            _ticketService = ticketService;
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

                // Can't be awaited here, ApplyQueryAttributes is not async. LoadPriceAsync handles its own errors.
                _ = LoadPriceAsync();
            }
        }

        private static string FormatPrice(decimal? price)
        {
            return price is null ? "..." : $"€ {price:F2}".Replace(".", ",");
        }

        private async Task LoadPriceAsync()
        {
            Price = null;
            IsBusy = true;
            try
            {
                Price = await _ticketService.CalculatePriceAsync(CurrentRequest.PassengerCount, CurrentRequest.IsRoundTrip);
            }
            catch (ApiException ex)
            {
                await Application.Current.MainPage.DisplayAlert("Fout", ex.Message, "OK");
            }
            catch (HttpRequestException)
            {
                // The phone can't reach the backend (no wifi, wrong network, API not running).
                await Application.Current.MainPage.DisplayAlert("Fout",
                    "We kunnen de server niet bereiken. Heeft u verbinding met het internet?", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task PayAsync()
        {
            await Application.Current.MainPage.DisplayAlert("Betaling", "Betaling succesvol ontvangen!", "OK");
        }
    }
}