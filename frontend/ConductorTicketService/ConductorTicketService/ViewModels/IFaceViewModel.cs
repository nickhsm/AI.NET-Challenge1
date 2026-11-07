using System.Windows.Input;
using ConductorTicketService.Models;

namespace ConductorTicketService.ViewModels;

public interface IFaceViewModel
{
    public bool HasStatusMessage { get; }
    public string? StatusMessage { get; }
    public ImageSource? PhotoPreview { get; set; }

    // True while waiting for the API: shows the loading spinner.
    public bool IsBusy { get; }
    public bool IsNotBusy { get; }

    public bool HasRequestResult { get;  }
    public CheckFaceResult? RequestResult { get; set; }

    public bool HasTicketsResult { get; }
    public IEnumerable<TicketListItemResult> TicketListItemResult { get; set; }

    ICommand ReturnCommand { get; set; }
    ICommand TakePhotoCommand { get; set; }
    ICommand ConsumeTicketCommand { get; set; }
}