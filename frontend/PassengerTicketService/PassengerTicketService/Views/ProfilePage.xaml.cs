using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class ProfilePage : ContentPage
{
	private readonly IProfileViewModel _viewModel;

	public ProfilePage(IProfileViewModel viewModel)
	{
		InitializeComponent();
		_viewModel = viewModel;
		BindingContext = viewModel;
	}

	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await _viewModel.LoadProfileAsync();
	}
}
