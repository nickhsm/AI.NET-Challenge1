using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class LoginPage : ContentPage
{
    private readonly ILoginViewModel _viewModel;

    public LoginPage(ILoginViewModel viewModel)
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
