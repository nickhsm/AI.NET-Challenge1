using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class ShoppingCartPage : ContentPage
{
	public ShoppingCartPage(IShoppingCartViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}