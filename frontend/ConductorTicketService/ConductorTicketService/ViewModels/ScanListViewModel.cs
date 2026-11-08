using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class ScanListViewModel : ObservableObject, IScanListViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IAuthService _authService;

    public ScanListViewModel(INavigationService navigationService, IAuthService authService)
    {
        BindCommands();
        _navigationService = navigationService;
        _authService = authService;
    }

    public ICommand ScanQrCodeCommand { get; set; }
    public ICommand ScanFaceCommand { get; set; }
    public ICommand LogoutCommand { get; set; }
    public ICommand AccountCommand { get; set; }

    private void BindCommands()
    {
        ScanQrCodeCommand = new AsyncRelayCommand(GoToScanQrCodeAsync);
        ScanFaceCommand = new AsyncRelayCommand(GoToScanFaceAsync);
        LogoutCommand = new AsyncRelayCommand(LogoutAsync);
        AccountCommand = new AsyncRelayCommand(GoToAccountAsync);
    }

    private async Task GoToAccountAsync()
    {
        await _navigationService.NavigateToAccountPageAsync();
    }

    private async Task GoToScanQrCodeAsync()
    {
        await _navigationService.NavigateToScanQrCodePageAsync();
    }

    private async Task GoToScanFaceAsync()
    {
        await _navigationService.NavigateToScanFacePageAsync();
    }

    private async Task LogoutAsync()
    {
        // Remove the token, then go to the login page with an absolute route,
        // so "back" can't return to the scan page.
        _authService.Logout();
        await _navigationService.NavigateToLoginPageAsync();
    }
}
