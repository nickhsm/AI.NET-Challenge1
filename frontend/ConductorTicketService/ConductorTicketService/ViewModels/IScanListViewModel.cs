using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IScanListViewModel
{
    ICommand ScanQrCodeCommand { get; set; }
    // ICommand ScanFaceCommand { get; set; }
    // ICommand ScanIdCommand { get; set; }
}