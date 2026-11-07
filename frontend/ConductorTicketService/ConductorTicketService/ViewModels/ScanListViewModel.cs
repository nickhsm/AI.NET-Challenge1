using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class ScanListViewModel : ObservableObject, IScanListViewModel
{
    private INavigationService _navigationService;
    private IAuthService _authService;

    public ScanListViewModel(INavigationService navigationService, IAuthService authService)
    {
        BindCommands();
        _navigationService = navigationService;
        _authService = authService;
    }

    public ICommand ScanQrCodeCommand { get; set; }
    public ICommand ScanFaceCommand { get; set; }
    public ICommand LogoutCommand { get; set; }

    private void BindCommands()
    {
        ScanQrCodeCommand = new AsyncRelayCommand(GoToScanQrCode);
        ScanFaceCommand = new AsyncRelayCommand(GoToScanFace);
        LogoutCommand = new AsyncRelayCommand(Logout);
    }

    private async Task GoToScanQrCode()
    {
        await _navigationService.NavigateToScanQrCodePageAsync();
    }

    private async Task GoToScanFace()
    {
        await _navigationService.NavigateToScanFacePageAsync();
    }

    private async Task Logout()
    {
        // Remove the token, then go to the login page with an absolute route,
        // so "back" can't return to the scan page.
        _authService.Logout();
        await _navigationService.NavigateToLoginPageAsync();
    }
}
