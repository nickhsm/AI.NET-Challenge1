using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class TicketPage : ContentPage
{
	public TicketPage(ITicketViewModel viewModel)
	{
		InitializeComponent();		
		BindingContext = viewModel;
	}
}