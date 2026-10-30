using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class ScanListPage : ContentPage
{
    public ScanListPage(IScanListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}