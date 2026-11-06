using PassengerTicketService.Models;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IYourTravelViewModel
    {
        TravelRequest CurrentRequest { get; }
        ICommand ConfirmTravelCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand SelectSingleTripCommand { get; }
        ICommand SelectRoundTripCommand { get; }
    }
}