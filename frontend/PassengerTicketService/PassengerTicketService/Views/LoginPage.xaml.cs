using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(ILoginViewModel loginViewModel)
	{
		InitializeComponent();
		BindingContext = loginViewModel;
	}
}