using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class ScanListViewModel : ObservableObject, IScanListViewModel
{
    private INavigationService _navigationService;

    public ScanListViewModel(INavigationService navigationService)
    {
        BindCommands();
        _navigationService = navigationService;
    }

    public ICommand ScanQrCodeCommand { get; set; }
    public ICommand ScanFaceCommand { get; set; }
    public ICommand ScanIdCommand { get; set; }

    private void BindCommands()
    {
        ScanQrCodeCommand = new AsyncRelayCommand(GoToScanQrCode);
        ScanFaceCommand = new AsyncRelayCommand(GoToScanFace);
        ScanIdCommand = new AsyncRelayCommand(GoToScanId);
    }

    private async Task GoToScanQrCode()
    {
        await _navigationService.NavigateToScanQrCodePageAsync();
    }

    private async Task GoToScanFace()
    {
        await _navigationService.NavigateToScanFacePageAsync();
    }

    private async Task GoToScanId()
    {
        await _navigationService.NavigateToScanIdPageAsync();
    }
}