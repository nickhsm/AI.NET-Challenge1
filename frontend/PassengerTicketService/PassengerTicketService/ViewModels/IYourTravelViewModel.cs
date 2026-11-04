using PassengerTicketService.Models;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IYourTravelViewModel
    {
        TravelRequest CurrentRequest { get; set; }
        ICommand ConfirmTravelCommand { get; set; }
        ICommand NavigateBackCommand { get; set; }
    }
}