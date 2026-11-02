using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class IdViewModel : ObservableObject, IIdViewModel
{
    private INavigationService _navigationService;

    public IdViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        Binding();
    }

    public ICommand ReturnCommand { get; set; }

    private void Binding()
    {
        ReturnCommand = new AsyncRelayCommand(GoBack);
    }

    private async Task GoBack()
    {
        await _navigationService.NavigateBackAsync();
    }
}