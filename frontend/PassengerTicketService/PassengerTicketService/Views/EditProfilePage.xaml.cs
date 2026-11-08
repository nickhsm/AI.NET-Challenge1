using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class EditProfilePage : ContentPage
{
	private readonly IEditProfileViewModel _viewModel;

	public EditProfilePage(IEditProfileViewModel viewModel)
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
