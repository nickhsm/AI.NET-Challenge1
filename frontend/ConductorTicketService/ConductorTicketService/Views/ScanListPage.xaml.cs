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