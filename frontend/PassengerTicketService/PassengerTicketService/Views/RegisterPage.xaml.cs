using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(IRegisterViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}