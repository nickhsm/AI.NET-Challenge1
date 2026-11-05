using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class YourTravelPage : ContentPage
{
	public YourTravelPage(IYourTravelViewModel viewModel)
	{
		InitializeComponent();		
		BindingContext = viewModel;
	}
}