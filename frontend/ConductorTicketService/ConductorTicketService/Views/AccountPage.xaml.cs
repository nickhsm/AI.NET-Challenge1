using ConductorTicketService.ViewModels;

namespace ConductorTicketService.Views;

public partial class AccountPage : ContentPage
{
    private readonly IAccountViewModel _viewModel;

    public AccountPage(IAccountViewModel viewModel)
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
