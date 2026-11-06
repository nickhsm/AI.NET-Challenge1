using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace PassengerTicketService.ViewModels
{
    public interface IHomeViewModel
    {
        string StartStation { get; set; }
        string EndStation { get; set; }
        ICommand BuyTicketCommand { get; }

        ICommand NavigateHomeCommand { get; }
        ICommand NavigateTicketsCommand { get; }
        ICommand NavigateProfileCommand { get; }
    }
}
