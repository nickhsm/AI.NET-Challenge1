using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class QrCodeViewModel : ObservableObject, IQrCodeViewModel
{
    private INavigationService _navigationService;

    public QrCodeViewModel(INavigationService navigationService)
    {
        BindCommands();
        _navigationService = navigationService;
    }

    public ICommand ReturnCommand { get; set; }

    private void BindCommands()
    {
        ReturnCommand = new AsyncRelayCommand(GoBack);
    }

    private async Task GoBack()
    {
        await _navigationService.NavigateBackAsync();
    }
}