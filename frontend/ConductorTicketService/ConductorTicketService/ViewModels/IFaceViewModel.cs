using System.Windows.Input;

namespace ConductorTicketService.ViewModels;

public interface IFaceViewModel
{
    public bool HasStatusMessage { get; }
    public string? StatusMessage { get; }
    public ImageSource? PhotoPreview { get; set; }

    ICommand ReturnCommand { get; set; }
    ICommand TakePhotoCommand { get; set; }
}