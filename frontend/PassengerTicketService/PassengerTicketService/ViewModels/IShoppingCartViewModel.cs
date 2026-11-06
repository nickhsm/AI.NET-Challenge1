using PassengerTicketService.Models;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IShoppingCartViewModel
    {
        TravelRequest CurrentRequest { get; }

        string ExpirationDateDisplay { get; }
        string ReturnExpirationDateDisplay { get; }
        string PassengerCountDisplay { get; }

        string OutboundPriceDisplay { get; }
        string ReturnPriceDisplay { get; }
        string TotalPriceDisplay { get; }

        ICommand PayCommand { get; }
        ICommand CancelCommand { get; }
        ICommand NavigateBackCommand { get; }
    }
}