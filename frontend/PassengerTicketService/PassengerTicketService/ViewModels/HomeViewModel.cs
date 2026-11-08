using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PassengerTicketService.Models;
using PassengerTicketService.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public partial class HomeViewModel : ObservableObject, IHomeViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IApiService _apiService;

        private CancellationTokenSource? _startSearchCts;
        private CancellationTokenSource? _endSearchCts;

        // Generatie-tellers: verhogen bij elke nieuwe zoekopdracht én bij elke selectie.
        // Een zoekopdracht mag alleen state updaten als zijn generatie nog actueel is.
        private int _startSearchGeneration;
        private int _endSearchGeneration;

        [ObservableProperty]
        private string startStationText = string.Empty;

        [ObservableProperty]
        private string endStationText = string.Empty;

        public ObservableCollection<StationDto> StartSuggestions { get; } = new();
        public ObservableCollection<StationDto> EndSuggestions { get; } = new();

        [ObservableProperty]
        private StationDto? selectedStartStation;

        [ObservableProperty]
        private StationDto? selectedEndStation;

        [ObservableProperty]
        private bool isStartSuggestionsVisible;

        [ObservableProperty]
        private bool isEndSuggestionsVisible;

        [ObservableProperty]
        private DateTime selectedDate = DateTime.Today;
        public DateTime MinimumDate => DateTime.Today;

        public string DateDisplayText
        {
            get
            {
                if (SelectedDate.Date == DateTime.Today)
                    return "vandaag";
                return SelectedDate.ToString("dd/MM/yyyy");
            }
        }

        public ICommand BuyTicketCommand { get; private set; } = null!;
        public ICommand NavigateHomeCommand { get; private set; } = null!;
        public ICommand NavigateTicketsCommand { get; private set; } = null!;
        public ICommand NavigateProfileCommand { get; private set; } = null!;
        public ICommand SwapStationsCommand { get; private set; } = null!;

        public HomeViewModel(INavigationService navigationService, IApiService apiService)
        {
            _navigationService = navigationService;
            _apiService = apiService;
            BindCommands();
        }

        private void BindCommands()
        {
            BuyTicketCommand = new AsyncRelayCommand(GoToYourTravelPage);
            NavigateTicketsCommand = new AsyncRelayCommand(_navigationService.NavigateToTicketsPageAsync);
            NavigateHomeCommand = new RelayCommand(() => { });
            NavigateProfileCommand = new AsyncRelayCommand(_navigationService.NavigateToProfilePageAsync);
            SwapStationsCommand = new RelayCommand(SwapStations);
        }

        partial void OnSelectedDateChanged(DateTime value)
        {
            OnPropertyChanged(nameof(DateDisplayText));
        }

        // ---------- TEXT CHANGED (gebruiker typt of control synct tekst) ----------

        partial void OnStartStationTextChanged(string value)
        {
            // Als de nieuwe tekst exact overeenkomt met het reeds geselecteerde station,
            // is dit een programmatische sync na selectie -> niets doen.
            if (SelectedStartStation != null && SelectedStartStation.Name == value)
                return;

            // Gebruiker wijzigt de tekst -> selectie vervalt.
            SelectedStartStation = null;

            var generation = ++_startSearchGeneration;
            _ = PerformStartSearchAsync(value, generation);
        }

        partial void OnEndStationTextChanged(string value)
        {
            if (SelectedEndStation != null && SelectedEndStation.Name == value)
                return;

            SelectedEndStation = null;

            var generation = ++_endSearchGeneration;
            _ = PerformEndSearchAsync(value, generation);
        }

        // ---------- SELECTIE GEWIJZIGD ----------

        partial void OnSelectedStartStationChanged(StationDto? value)
        {
            if (value == null) return;

            // Invalideer ALLE lopende zoekopdrachten (ook die van het eindstation),
            // zodat geen enkele stale search nog de dropdowns kan openen.
            _startSearchGeneration++;
            _endSearchGeneration++;
            _startSearchCts?.Cancel();
            _endSearchCts?.Cancel();

            // Sluit én leeg de lijsten: control kan niet heropenen met oude items.
            IsStartSuggestionsVisible = false;
            IsEndSuggestionsVisible = false;
            StartSuggestions.Clear();
            EndSuggestions.Clear();
        }

        partial void OnSelectedEndStationChanged(StationDto? value)
        {
            if (value == null) return;

            _endSearchGeneration++;
            _startSearchGeneration++;
            _endSearchCts?.Cancel();
            _startSearchCts?.Cancel();

            IsEndSuggestionsVisible = false;
            IsStartSuggestionsVisible = false;
            EndSuggestions.Clear();
            StartSuggestions.Clear();
        }

        // ---------- ZOEKOPDRACHTEN ----------

        private async Task PerformStartSearchAsync(string query, int generation)
        {
            _startSearchCts?.Cancel();
            var cts = new CancellationTokenSource();
            _startSearchCts = cts;
            var token = cts.Token;

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (generation == _startSearchGeneration)
                {
                    StartSuggestions.Clear();
                    IsStartSuggestionsVisible = false;
                }
                return;
            }

            try
            {
                await Task.Delay(300, token);
                var results = await _apiService.SearchStationsAsync(query);

                // Generatie-check: is deze zoekopdracht nog relevant?
                if (generation != _startSearchGeneration) return;
                token.ThrowIfCancellationRequested();

                // Niet heropenen als de zoekopdracht overeenkomt met de selectie.
                if (SelectedStartStation != null && SelectedStartStation.Name == query)
                {
                    IsStartSuggestionsVisible = false;
                    return;
                }

                StartSuggestions.Clear();
                foreach (var station in results) StartSuggestions.Add(station);
                IsStartSuggestionsVisible = StartSuggestions.Any();
            }
            catch (OperationCanceledException) { /* verwacht */ }
            catch (Exception ex)
            {
                if (generation == _startSearchGeneration)
                    IsStartSuggestionsVisible = false;
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
        }

        private async Task PerformEndSearchAsync(string query, int generation)
        {
            _endSearchCts?.Cancel();
            var cts = new CancellationTokenSource();
            _endSearchCts = cts;
            var token = cts.Token;

            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            {
                if (generation == _endSearchGeneration)
                {
                    EndSuggestions.Clear();
                    IsEndSuggestionsVisible = false;
                }
                return;
            }

            try
            {
                await Task.Delay(300, token);
                var results = await _apiService.SearchStationsAsync(query);

                if (generation != _endSearchGeneration) return;
                token.ThrowIfCancellationRequested();

                if (SelectedEndStation != null && SelectedEndStation.Name == query)
                {
                    IsEndSuggestionsVisible = false;
                    return;
                }

                EndSuggestions.Clear();
                foreach (var station in results) EndSuggestions.Add(station);
                IsEndSuggestionsVisible = EndSuggestions.Any();
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                if (generation == _endSearchGeneration)
                    IsEndSuggestionsVisible = false;
                System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            }
        }

        // ---------- SWAP ----------

        private void SwapStations()
        {
            // Invalideer lopende searches
            _startSearchGeneration++;
            _endSearchGeneration++;
            _startSearchCts?.Cancel();
            _endSearchCts?.Cancel();

            var tempText = StartStationText;
            StartStationText = EndStationText;
            EndStationText = tempText;

            var tempObj = SelectedStartStation;
            SelectedStartStation = SelectedEndStation;
            SelectedEndStation = tempObj;
        }

        // ---------- NAVIGATIE ----------

        private async Task GoToYourTravelPage()
        {
            // 1. Controleer of beide stations zijn geselecteerd
            if (SelectedStartStation == null || SelectedEndStation == null)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Fout", "Selecteer a.u.b. geldige stations uit de lijst.", "OK");
                return;
            }

            // 2. Controleer of het om verschillende stations gaat (vergelijk op Id)
            if (SelectedStartStation.Id == SelectedEndStation.Id)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Fout", "Vertrek- en aankomststation mogen niet hetzelfde zijn.", "OK");
                return;
            }

            // 3. Controleer of de getypte tekst overeenkomt met de geselecteerde stations
            //    (voorkomt dat de gebruiker een station typt zonder het te selecteren)
            if (!string.Equals(StartStationText?.Trim(), SelectedStartStation.Name, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(EndStationText?.Trim(), SelectedEndStation.Name, StringComparison.OrdinalIgnoreCase))
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Fout", "Selecteer a.u.b. geldige stations uit de lijst.", "OK");
                return;
            }

            var request = new TravelRequest
            {
                StartStationId = SelectedStartStation.Id,
                StartStation = SelectedStartStation.Name,
                EndStationId = SelectedEndStation.Id,
                EndStation = SelectedEndStation.Name,
                PassengerCount = 1,
                IsRoundTrip = false,
                ExpirationDate = SelectedDate,
            };

            if (request.ExpirationDate < DateTime.Today)
                request.ExpirationDate = DateTime.Today;

            await _navigationService.NavigateToYourTravelPageAsync(request);
        }
    }
}