using ConductorTicketService.Views;

namespace ConductorTicketService.Services;

public class NavigationService : INavigationService
{
    private readonly INavigation _navigation;
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _navigation = Shell.Current?.Navigation;
    }

    public async Task NavigateToScanQrCodePageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<QrCodePage>());
    }

    public async Task NavigateToScanFacePageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<FacePage>());
    }

    public async Task NavigateBackAsync()
    {
        if (_navigation.NavigationStack.Count > 1)
        {
            await _navigation.PopAsync();
        }
        else
        {
            throw new InvalidOperationException("There are no pages left to navigate back to.");
        }
    }

    public async Task NavigateToMainPageAsync()
    {
        await Shell.Current.GoToAsync("//MainPage");
    }

    public async Task NavigateToLoginPageAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() => Shell.Current.GoToAsync("//LoginPage"));
    }

    // Pushed on top of the login page (not a Shell route), so the back button returns to login.
    public async Task NavigateToRegisterPageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<RegisterPage>());
    }

    // Pushed on top of the scan page, so the back button returns there.
    public async Task NavigateToAccountPageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<AccountPage>());
    }

    // Pushed on top of the account page, so the back button returns there.
    public async Task NavigateToChangePasswordPageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<ChangePasswordPage>());
    }
}