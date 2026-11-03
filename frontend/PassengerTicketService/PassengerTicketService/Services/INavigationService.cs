using System;
using System.Collections.Generic;
using System.Text;

namespace PassengerTicketService.Services
{
    public interface INavigationService
    {
        Task NavigateToYourTravelPageAsync();
        Task NavigateToShoppingCartPageAsync();
        Task NavigateToTicketsPageAsync();
        Task NavigateBackAsync();
    }
}
