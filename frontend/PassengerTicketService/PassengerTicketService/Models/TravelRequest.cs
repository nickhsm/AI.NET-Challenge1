using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace PassengerTicketService.Models
{
    public partial class TravelRequest : ObservableObject
    {
        [ObservableProperty]
        private string startStation;

        [ObservableProperty]
        private string endStation;

        [ObservableProperty]
        private int passengerCount;

        [ObservableProperty]
        private bool isRoundTrip;

        [ObservableProperty]
        private DateOnly expirationDate = DateOnly.FromDateTime(DateTime.Today);
    }
}
