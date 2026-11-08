using System.Windows.Input;
using ConductorTicketService.Models;

namespace ConductorTicketService.ViewModels;

public interface IFaceViewModel
{
    bool HasStatusMessage { get; }
    string? StatusMessage { get; }
    ImageSource? PhotoPreview { get; set; }

    // True while waiting for the API: shows the loading spinner.
    bool IsBusy { get; }
    bool IsNotBusy { get; }

    bool HasRequestResult { get;  }
    CheckFaceResult? RequestResult { get; set; }

    bool HasTicketsResult { get; }
    IEnumerable<TicketListItemResult> TicketListItemResult { get; set; }

    ICommand TakePhotoCommand { get; set; }
    ICommand ConsumeTicketCommand { get; set; }
}