using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class StartPage : ContentPage
{
    public StartPage(IStartViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}