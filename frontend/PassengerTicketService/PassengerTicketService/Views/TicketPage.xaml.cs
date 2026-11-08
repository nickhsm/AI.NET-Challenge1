using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class TicketPage : ContentPage
{
	private readonly ITicketViewModel _viewModel;

	public TicketPage(ITicketViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadTicketsAsync();
	}
}