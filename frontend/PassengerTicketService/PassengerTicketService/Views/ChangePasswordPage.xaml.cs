using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage(IChangePasswordViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }
}