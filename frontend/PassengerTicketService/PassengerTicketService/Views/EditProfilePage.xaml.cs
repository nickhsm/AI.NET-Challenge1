using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class EditProfilePage : ContentPage
{
	public EditProfilePage(IEditProfileViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
    }
}