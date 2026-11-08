using PassengerTicketService.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IHomeViewModel
    {
        // Voor de autocomplete (deze hebben we nu)
        string StartStationText { get; set; }
        string EndStationText { get; set; }
        StationDto? SelectedStartStation { get; set; }
        StationDto? SelectedEndStation { get; set; }
        ObservableCollection<StationDto> StartSuggestions { get; }
        ObservableCollection<StationDto> EndSuggestions { get; }
        bool IsStartSuggestionsVisible { get; set; }
        bool IsEndSuggestionsVisible { get; set; }
        string SearchErrorMessage { get; }

        // Voor de datum
        DateTime SelectedDate { get; set; }
        DateTime MinimumDate { get; }
        string DateDisplayText { get; }

        // Commando's
        ICommand BuyTicketCommand { get; }
        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
        ICommand NavigateProfileCommand { get; }
        ICommand SwapStationsCommand { get; }
    }
}