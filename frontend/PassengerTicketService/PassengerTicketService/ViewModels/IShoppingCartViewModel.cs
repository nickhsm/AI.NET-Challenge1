using PassengerTicketService.Models;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IShoppingCartViewModel
    {
        TravelRequest CurrentRequest { get; set; }

        string ExpirationDateDisplay { get; }
        string PassengerCountDisplay { get; }

        string OutboundPriceDisplay { get; }
        string ReturnPriceDisplay { get; }
        string TotalPriceDisplay { get; }

        ICommand PayCommand { get; }
        ICommand CancelCommand { get; }
        ICommand NavigateBackCommand { get; }
    }
}