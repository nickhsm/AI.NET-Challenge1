using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class FacePage : ContentPage
{
    public FacePage(IFaceViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
    }
}