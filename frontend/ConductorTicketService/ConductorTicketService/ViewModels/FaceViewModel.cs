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
    private readonly ITicketService _ticketService;

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

    // Inverse of IsBusy, so the XAML can bind IsEnabled without a converter.
    public bool IsNotBusy => !IsBusy;
    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (SetProperty(ref _isBusy, value))
            {
                OnPropertyChanged(nameof(IsNotBusy));
            }
        }
    }

    public bool HasRequestResult => RequestResult is not null;
    private CheckFaceResult? _checkFaceResult;

    public CheckFaceResult? RequestResult
    {
        get => _checkFaceResult;
        set
        {
            if (SetProperty(ref _checkFaceResult, value))
            {
                OnPropertyChanged(nameof(HasRequestResult));
            }
        }
    }

    public bool HasTicketsResult => TicketListItemResult.Any();
    private IEnumerable<TicketListItemResult> _ticketListItemResult = [];

    public IEnumerable<TicketListItemResult> TicketListItemResult
    {
        get => _ticketListItemResult;
        set
        {
            if (SetProperty(ref _ticketListItemResult, value))
            {
                OnPropertyChanged(nameof(HasTicketsResult));
            }
        }
    }

    public FaceViewModel(INavigationService navigationService, IFaceCheckService faceCheckService,
        ITicketService ticketService)
    {
        BindCommands();
        _navigationService = navigationService;
        _faceCheckService = faceCheckService;
        _ticketService = ticketService;
    }

    public ICommand ReturnCommand { get; set; }
    public ICommand TakePhotoCommand { get; set; }
    public ICommand ConsumeTicketCommand { get; set; }

    private void BindCommands()
    {
        ReturnCommand = new AsyncRelayCommand(GoBack);
        TakePhotoCommand = new AsyncRelayCommand(TakePhotoAsync);
        ConsumeTicketCommand = new AsyncRelayCommand<TicketListItemResult>(ConsumeTicketAsync);
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
            RequestResult = null;
            TicketListItemResult = [];

            // Only now: the camera app is closed and we start waiting for the API.
            IsBusy = true;

            await using var fileStream = await fileResult.OpenReadAsync();
            var result = await _faceCheckService.CheckFaceAsync(fileStream);

            if (result is null)
            {
                StatusMessage = "Passagier niet herkend";
                return;
            }

            var resultTickets = await _ticketService.GetTicketsByFacePersonId(result.FacePersonId);

            RequestResult = result;
            TicketListItemResult = resultTickets;

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
        catch (PassengerNotFoundException)
        {
            StatusMessage =
                "Passagier niet gevonden, waarschijnlijk heeft hij zijn gezicht niet geregistreerd. Scan de QR-code van de ticket.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ConsumeTicketAsync(TicketListItemResult? ticket)
    {
        if (ticket is null) return;

        IsBusy = true;
        try
        {
            await _ticketService.ConsumeTicket(ticket.Id);

            // Alles is succesvol afgerond, de conducteur kan naar de volgende passagier.
            PhotoPreview = null;
            RequestResult = null;
            TicketListItemResult = [];

            StatusMessage = "Ticket gebruikt. U kan de volgende passagier scannen.";
        }
        catch (TicketNotConsumableException)
        {
            StatusMessage = "Dit ticket kan niet gebruikt worden: het is al gebruikt of niet geldig vandaag.";
        }
        catch (TicketNotFoundException)
        {
            StatusMessage = "Dit ticket kan niet worden gevonden.";
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
        finally
        {
            IsBusy = false;
        }
    }

    private async Task ConsumeTicketAsync(TicketListItemResult? ticket)
    {
        if (ticket is null) return;

        IsBusy = true;
        try
        {
            await _ticketService.ConsumeTicket(ticket.Id);

            // Alles is succesvol afgerond, de conducteur kan naar de volgende passagier.
            PhotoPreview = null;
            RequestResult = null;
            TicketListItemResult = [];

            StatusMessage = "Ticket gebruikt. U kan de volgende passagier scannen.";
        }
        catch (TicketNotConsumableException)
        {
            StatusMessage = "Dit ticket kan niet gebruikt worden: het is al gebruikt of niet geldig vandaag.";
        }
        catch (TicketNotFoundException)
        {
            StatusMessage = "Dit ticket kan niet worden gevonden.";
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
        finally
        {
            IsBusy = false;
        }
    }
}