using PassengerTicketService.ViewModels;

namespace PassengerTicketService.Views;

public partial class StartPage : ContentPage
{
    private readonly IStartViewModel _viewModel;

    public StartPage(IStartViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
    }

    // Runs every time the page becomes visible, also when the app starts.
    // A constructor can't await, so the "already logged in?" check happens here.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckAlreadyLoggedInAsync();
    }
}
