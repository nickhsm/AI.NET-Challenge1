using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class TicketViewModel : ObservableObject, ITicketViewModel
    {
        // Singleton van DI gebrunchnuukd voor de navbar te to the Home-page
        private readonly INavigationService _navigationService;
        private readonly ITicketService _ticketService;

        // All tickets from the backend. The tabs show a filtered part of this list.
        private List<PassengerTicketResult> _allTickets = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private ObservableCollection<PassengerTicketResult> displayedTickets = new();

        // True while waiting on the backend, shows the spinner.
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private bool isBusy = false;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasErrorMessage))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyMessage))]
        private string? errorMessage;

        public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        // Only say there are no tickets when we actually know that.
        public bool ShowEmptyMessage => !IsBusy && !HasErrorMessage && DisplayedTickets.Count == 0;

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

        public ICommand ShowActiveCommand { get; private set; } = null!;
        public ICommand ShowArchiveCommand { get; private set; } = null!;
        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;

        public TicketViewModel(INavigationService navigationService, ITicketService ticketService)
        {
            _navigationService = navigationService;
            _ticketService = ticketService;
            BindCommands();
            SwitchTab(true); // Start op 'Actief'
        }

        // Called on every OnAppearing, so a new purchase shows up when coming back to this page.
        public async Task LoadTicketsAsync()
        {
            ErrorMessage = null;
            IsBusy = true;
            try
            {
                _allTickets = await _ticketService.GetMyTicketsAsync();

                // Stay on the tab the passenger had open.
                SwitchTab(IsActiveTabSelected);
            }
            catch (ApiException ex)
            {
                ErrorMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
                Debug.WriteLine(ex);
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                ErrorMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void BindCommands()
        {
            ShowActiveCommand = new RelayCommand(() => SwitchTab(true));
            ShowArchiveCommand = new RelayCommand(() => SwitchTab(false));
            // Navbar Home-ikoon → to the Home-page
            NavigateHomeCommand = new AsyncRelayCommand(_navigationService.NavigateToHomePageAsync);
            //Het doet niets.
            //Dit is door de navbar.
            NavigateTicketsCommand = new RelayCommand(() => { });
            NavigateProfileCommand = new AsyncRelayCommand(_navigationService.NavigateToProfilePageAsync);
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

            var filtered = _allTickets.Where(t =>
                showActive
                ? !t.Used && t.Date >= today // Actief
                : t.Used || t.Date < today   // Archief
            ).OrderByDescending(t => t.Date).ToList();

            DisplayedTickets = new ObservableCollection<PassengerTicketResult>(filtered);
        }
    }
}