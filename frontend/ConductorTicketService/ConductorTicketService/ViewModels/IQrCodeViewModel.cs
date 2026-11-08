using System.Windows.Input;
using ConductorTicketService.Models;

namespace ConductorTicketService.ViewModels;

public interface IQrCodeViewModel
{
    bool HasStatusMessage { get; }
    string? StatusMessage { get; }

    // Bound to the camera view: false = camera stops looking for codes.
    bool IsDetecting { get; set; }

    // True while waiting for the API: shows the loading spinner.
    bool IsBusy { get; }
    bool IsNotBusy { get; }

    bool HasRequestResult { get; }
    TicketQrCodeResult? RequestResult { get; set; }

    ICommand QrCodeScannedCommand { get; set; }
    ICommand ScanAgainCommand { get; set; }
    ICommand ConsumeTicketCommand { get; set; }

    Task CheckCameraPermissionAsync();
}
