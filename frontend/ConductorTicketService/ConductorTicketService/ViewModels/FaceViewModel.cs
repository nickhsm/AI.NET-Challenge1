using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class FaceViewModel : ObservableObject, IFaceViewModel
{
    private INavigationService _navigationService;

    // Deze is voor de API request
    private FileResult? _photoFile;

    private ImageSource? _photoPreview;
    public ImageSource? PhotoPreview
    {
        get => _photoPreview;
        set => SetProperty(ref _photoPreview, value);
    }

    public FaceViewModel(INavigationService navigationService)
    {
        BindCommands();
        _navigationService = navigationService;
    }

    public ICommand ReturnCommand { get; set; }
    public ICommand TakePhotoCommand { get; set; }

    private void BindCommands()
    {
        ReturnCommand = new AsyncRelayCommand(GoBack);
        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
    }

    private async Task GoBack()
    {
        await _navigationService.NavigateBackAsync();
    }

    private async Task TakePhotoAsync()
    {
        FileResult? fileResult = await MediaPicker.Default.CapturePhotoAsync();
        if (fileResult is null)
        {
            return;
        }

        PhotoPreview = ImageSource.FromFile(fileResult.FullPath);
        _photoFile = fileResult;
    }
}