using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class TicketViewModel : ObservableObject, ITicketViewModel
    {
        // Simulatie van alle opgehaalde tickets uit de API
        private List<PurchaseDisplayModel> _allPurchases = new();

        [ObservableProperty]
        private ObservableCollection<PurchaseDisplayModel> displayedTickets = new();

        [ObservableProperty]
        private bool isActiveTabSelected = true;

        public ICommand ShowActiveCommand { get; }
        public ICommand ShowArchiveCommand { get; }

        public Color ActiveTabColor => IsActiveTabSelected ? Color.FromArgb("#5AA2F0") : Colors.Transparent;
        public Color ArchiveTabColor => !IsActiveTabSelected ? Color.FromArgb("#5AA2F0") : Colors.Transparent;
        public Color ActiveTabTextColor => IsActiveTabSelected ? Colors.Black : Colors.White;
        public Color ArchiveTabTextColor => !IsActiveTabSelected ? Colors.Black : Colors.White;

        public TicketViewModel()
        {
            ShowActiveCommand = new RelayCommand(() => SwitchTab(true));
            ShowArchiveCommand = new RelayCommand(() => SwitchTab(false));

            LoadMockData();
            SwitchTab(true); // Start op 'Actief'
        }

        private void SwitchTab(bool showActive)
        {
            IsActiveTabSelected = showActive;

            // Update UI kleuren
            OnPropertyChanged(nameof(ActiveTabColor));
            OnPropertyChanged(nameof(ArchiveTabColor));
            OnPropertyChanged(nameof(ActiveTabTextColor));
            OnPropertyChanged(nameof(ArchiveTabTextColor));

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
            // In het echt haal je dit uit je backend Ticket entity (ticket.Used)
            // Voor de demo gebruiken we de Guid als indicator (oneven = gebruikt)
            return ticket.GuidQrCode.GetHashCode() % 2 == 0;
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