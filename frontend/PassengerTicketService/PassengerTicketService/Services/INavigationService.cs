using PassengerTicketService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassengerTicketService.Services
{
    public interface INavigationService
    {
        Task NavigateToYourTravelPageAsync(TravelRequest request);
        Task NavigateToShoppingCartPageAsync(TravelRequest request);
        Task NavigateToTicketsPageAsync();
        Task NavigateToHomePageAsync();
        Task NavigateToProfilePageAsync();
        Task NavigateToRegisterPageAsync();
        Task NavigateBackAsync();
    }
}
