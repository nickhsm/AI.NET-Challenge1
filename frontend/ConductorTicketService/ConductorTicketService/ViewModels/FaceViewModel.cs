using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class FaceViewModel : ObservableObject, IFaceViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IFaceCheckService _faceCheckService;

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    private string? _statusMessage;

    public string? StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (SetProperty(ref _statusMessage, value))
            {
                OnPropertyChanged(nameof(HasStatusMessage));
            }
        }
    }

    private ImageSource? _photoPreview;

    public ImageSource? PhotoPreview
    {
        get => _photoPreview;
        set => SetProperty(ref _photoPreview, value);
    }

    public FaceViewModel(INavigationService navigationService, IFaceCheckService faceCheckService)
    {
        BindCommands();
        _navigationService = navigationService;
        _faceCheckService = faceCheckService;
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
        StatusMessage = null;

        try
        {
            FileResult? fileResult = await MediaPicker.Default.CapturePhotoAsync();
            if (fileResult is null)
            {
                return;
            }

            PhotoPreview = ImageSource.FromFile(fileResult.FullPath);

            await using var fileStream = await fileResult.OpenReadAsync();
            var result = await _faceCheckService.CheckFaceAsync(fileStream);

            if (result is null)
            {
                StatusMessage = "Passagier niet herkend";
            }

            Debug.WriteLine($"{result?.FirstName} - {result?.HasTickets}");
        }
        catch (InvalidPhotoException)
        {
            StatusMessage = "Zorg ervoor dat er exact één gezicht op de foto staat.";
        }
        catch (ApiException ex)
        {
            StatusMessage = "Er is iets foutgelopen. Probeer het later opnieuw.";
            Debug.WriteLine(ex);
        }
        catch (HttpRequestException ex)
        {
            StatusMessage = "We kunnen de server niet bereiken. Heeft u verbinding met het internet?";
            Debug.WriteLine(ex);
        }
        catch (PermissionException)
        {
            StatusMessage = "Geef toestemming voor het gebruiken van de camera in de instellingen van uw smartphone.";
        }
    }
}