using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IQrCodeViewModel
{
    ICommand ReturnCommand { get; set; }
}