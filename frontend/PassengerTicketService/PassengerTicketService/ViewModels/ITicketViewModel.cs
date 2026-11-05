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

        Color ActiveTabColor { get; }
        Color ArchiveTabColor { get; }
        Color ActiveTabTextColor { get; }
        Color ArchiveTabTextColor { get; }
    }
}