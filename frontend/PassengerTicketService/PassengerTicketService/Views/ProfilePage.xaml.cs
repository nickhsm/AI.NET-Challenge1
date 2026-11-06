using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class ProfilePage : ContentPage
{
	public ProfilePage(IProfileViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}