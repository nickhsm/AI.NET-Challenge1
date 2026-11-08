using PassengerTicketService.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassengerTicketService.Services
{
    public interface IApiService
    {
        Task<IEnumerable<StationDto>> SearchStationsAsync(string query);
    }
}
