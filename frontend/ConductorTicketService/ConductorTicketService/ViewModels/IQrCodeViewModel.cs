using System.Windows.Input;
using ConductorTicketService.Models;

namespace ConductorTicketService.ViewModels;

public interface IQrCodeViewModel
{
    public bool HasStatusMessage { get; }
    public string? StatusMessage { get; }

    // Bound to the camera view: false = camera stops looking for codes.
    public bool IsDetecting { get; set; }

    public bool HasRequestResult { get; }
    public TicketQrCodeResult? RequestResult { get; set; }

    ICommand ReturnCommand { get; set; }
    ICommand QrCodeScannedCommand { get; set; }
    ICommand ScanAgainCommand { get; set; }
    ICommand ConsumeTicketCommand { get; set; }

    Task CheckCameraPermissionAsync();
}
