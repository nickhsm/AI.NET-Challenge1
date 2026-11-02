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

    public async Task NavigateToScanIdPageAsync()
    {
        await _navigation.PushAsync(_serviceProvider.GetRequiredService<IdPage>());
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
}