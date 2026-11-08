using PassengerTicketService.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface ITicketViewModel
    {
        ObservableCollection<PassengerTicketResult> DisplayedTickets { get; }

        // True while waiting on the backend (spinner)
        bool IsBusy { get; set; }

        string? ErrorMessage { get; set; }
        bool HasErrorMessage { get; }
        bool ShowEmptyMessage { get; }

        Task LoadTicketsAsync();

        bool IsActiveTabSelected { get; set; }
        ICommand ShowActiveCommand { get; }
        ICommand ShowArchiveCommand { get; }

        // Navbar-commands: Home-button to Home-page, FAB (train) to Ticket-page
        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
        ICommand NavigateProfileCommand { get; }

        Color ActiveTabColor { get; }
        Color ArchiveTabColor { get; }
        Color ActiveTabTextColor { get; }
        Color ArchiveTabTextColor { get; }
    }
}