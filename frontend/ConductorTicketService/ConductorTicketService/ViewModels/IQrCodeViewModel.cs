using System.Windows.Input;
using ConductorTicketService.Models;

namespace ConductorTicketService.ViewModels;

public interface IQrCodeViewModel
{
    public bool HasStatusMessage { get; }
    public string? StatusMessage { get; }

    // Bound to the camera view: false = camera stops looking for codes.
    public bool IsDetecting { get; set; }

    // True while waiting for the API: shows the loading spinner.
    public bool IsBusy { get; }
    public bool IsNotBusy { get; }

    public bool HasRequestResult { get; }
    public TicketQrCodeResult? RequestResult { get; set; }

    ICommand QrCodeScannedCommand { get; set; }
    ICommand ScanAgainCommand { get; set; }
    ICommand ConsumeTicketCommand { get; set; }

    Task CheckCameraPermissionAsync();
}
