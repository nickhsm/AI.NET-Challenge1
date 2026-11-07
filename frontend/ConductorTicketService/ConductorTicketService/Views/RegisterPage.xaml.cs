using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(IRegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
