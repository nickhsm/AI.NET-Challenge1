using PassengerTicketService.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface ITicketViewModel
    {
        ObservableCollection<PurchaseDisplayModel> DisplayedTickets { get; }

        bool IsActiveTabSelected { get; set; }
        ICommand ShowActiveCommand { get; }
        ICommand ShowArchiveCommand { get; }

        // Navbar-commands: Home-button to Home-page, FAB (train) to Ticket-page
        ICommand NavigateHomeCommand { get; set; }
        ICommand NavigateTicketsCommand { get; set; }

        Color ActiveTabColor { get; }
        Color ArchiveTabColor { get; }
        Color ActiveTabTextColor { get; }
        Color ArchiveTabTextColor { get; }
    }
}