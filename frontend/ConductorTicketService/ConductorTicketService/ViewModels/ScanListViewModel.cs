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
    // public ICommand ScanFaceCommand { get; set; }
    // public ICommand ScanIdCommand { get; set; }

    private void BindCommands()
    {
        ScanQrCodeCommand = new AsyncRelayCommand(GoToScanQrCode);
    }

    private async Task GoToScanQrCode()
    {
        await _navigationService.NavigateToScanQrCodePageAsync();
    }
}