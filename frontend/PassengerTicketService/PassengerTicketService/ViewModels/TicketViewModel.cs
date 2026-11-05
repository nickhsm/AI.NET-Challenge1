using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class TicketViewModel : ObservableObject, ITicketViewModel
    {
        // Singleton van DI gebrunchnuukd voor de navbar te to the Home-page
        private readonly INavigationService _navigationService;

        // Simulatie van alle opgehaalde tickets uit de API
        private List<PurchaseDisplayModel> _allPurchases = new();

        [ObservableProperty]
        private ObservableCollection<PurchaseDisplayModel> displayedTickets = new();

        [ObservableProperty]
        private bool isActiveTabSelected = true;

        // Observable → de bound XAML-colors refresh automatic when de tab switches
        // (de computed getters van vor never notified: OnPropertyChanged on a
        // non-observable getter is a nix-op, dus de tab-colors never refreshed)
        [ObservableProperty]
        private Color activeTabColor = Color.FromArgb("#5AA2F0");

        [ObservableProperty]
        private Color archiveTabColor = Color.FromArgb("#232325");

        [ObservableProperty]
        private Color activeTabTextColor = Color.FromArgb("#000000");

        [ObservableProperty]
        private Color archiveTabTextColor = Color.FromArgb("#FFFFFF");

        public ICommand ShowActiveCommand { get; }
        public ICommand ShowArchiveCommand { get; }
        public ICommand NavigateHomeCommand { get; set; }
        public ICommand NavigateTicketsCommand { get; set; }

        public TicketViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            ShowActiveCommand = new RelayCommand(() => SwitchTab(true));
            ShowArchiveCommand = new RelayCommand(() => SwitchTab(false));

            // Navbar FAB to te Ticket-pagina: already here → nix-op, otherwise te die
            // stack getsed a duplicate Ticket-page per tap
            NavigateTicketsCommand = new RelayCommand(() => { });

            // Navbar Home-ikoon → to the Home-page
            NavigateHomeCommand = new AsyncRelayCommand(_navigationService.NavigateToHomePageAsync);

            LoadMockData();
            SwitchTab(true); // Start op 'Actief'
        }

        private void SwitchTab(bool showActive)
        {
            IsActiveTabSelected = showActive;

            // Observable → de bound XAML-colors refresh automatic when de tab switches
            ActiveTabColor = showActive ? Color.FromArgb("#5AA2F0") : Color.FromArgb("#232325");
            ArchiveTabColor = showActive ? Color.FromArgb("#232325") : Color.FromArgb("#5AA2F0");
            ActiveTabTextColor = showActive ? Color.FromArgb("#000000") : Color.FromArgb("#FFFFFF");
            ArchiveTabTextColor = showActive ? Color.FromArgb("#FFFFFF") : Color.FromArgb("#000000");

            // Filter de tickets op basis van de API data
            var today = DateOnly.FromDateTime(DateTime.Today);

            var filtered = _allPurchases.Where(p =>
                showActive
                ? p.Tickets.All(t => !IsTicketUsed(t) && p.ExpirationDate >= today) // Actief
                : p.Tickets.Any(t => IsTicketUsed(t) || p.ExpirationDate < today)    // Archief
            ).OrderByDescending(p => p.PurchaseDate).ToList();

            DisplayedTickets = new ObservableCollection<PurchaseDisplayModel>(filtered);
        }

        // Helper om te checken of een ticket in de groep 'Used' is.
        // In je backend heb je 'Used'. Omdat de DTO dit mist, simuleren we het hier.
        private bool IsTicketUsed(TicketDisplayModel ticket)
        {
            // Simulatie: een ticket is 'gebruwd' when its travel date already passed.
            // De hash van het Guid is not deterministic per run: .NET randomiseiert per proces,
            // wat maakt de Actief-list empty in ~75% van de runs.
            // In de real app: take ticket.Used from de backend Ticket entity.
            var today = DateOnly.FromDateTime(DateTime.Today);
            return ticket.TravelDate < today;
        }

        // Tijdelijke data om de UI te testen
        private void LoadMockData()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            _allPurchases.Add(new PurchaseDisplayModel
            {
                ExpirationDate = today.AddDays(1),
                Tickets = new List<TicketDisplayModel>
                {
                    new() { JourneyType = "Heenreis", StationFrom = "MOL", StationTo = "GEEL", TravelDate = today, GuidQrCode = Guid.NewGuid() },
                    new() { JourneyType = "Terugreis", StationFrom = "GEEL", StationTo = "MOL", TravelDate = today, GuidQrCode = Guid.NewGuid() }
                }
            });

            _allPurchases.Add(new PurchaseDisplayModel
            {
                ExpirationDate = today.AddDays(-1), // Verlopen -> Archief
                PurchaseDate = DateTime.Now.AddDays(-2),
                Tickets = new List<TicketDisplayModel>
                {
                    new() { JourneyType = "Enkele reis", StationFrom = "MOL", StationTo = "GEEL", TravelDate = today.AddDays(-1), GuidQrCode = Guid.NewGuid() }
                }
            });
        }
    }
}