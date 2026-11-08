using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(IChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
