using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class YourTravelPage : ContentPage
{
	public YourTravelPage(IYourTravelViewModel viewModel)
	{
		InitializeComponent();		
		BindingContext = viewModel;
	}

    // Deze methodes worden aangeroepen als de gebruiker op de blokken klikt
    private void OnEnkelTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is IYourTravelViewModel vm)
        {
            vm.CurrentRequest.IsRoundTrip = false;
        }
    }

    private void OnHeenEnTerugTapped(object sender, TappedEventArgs e)
    {
        if (BindingContext is IYourTravelViewModel vm)
        {
            vm.CurrentRequest.IsRoundTrip = true;
        }
    }
}