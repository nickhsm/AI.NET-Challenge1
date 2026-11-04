using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class HomePage : ContentPage
{
	public HomePage(IHomeViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}