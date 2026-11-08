using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;
using ConductorTicketService.Services;

namespace ConductorTicketService.ViewModels;

public class QrCodeViewModel : ObservableObject, IQrCodeViewModel
{
    private readonly INavigationService _navigationService;
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

    private bool _isDetecting = true;

    public bool IsDetecting
    {
        get => _isDetecting;
        set => SetProperty(ref _isDetecting, value);
    }

    // Inverse of IsBusy, so the XAML can bind IsEnabled/IsVisible without a converter.
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
    private TicketQrCodeResult? _requestResult;

    public TicketQrCodeResult? RequestResult
    {
        get => _requestResult;
        set
        {
            if (SetProperty(ref _requestResult, value))
            {
                OnPropertyChanged(nameof(HasRequestResult));
            }
        }
    }

    public QrCodeViewModel(INavigationService navigationService, ITicketService ticketService)
    {
        BindCommands();
        _navigationService = navigationService;
        _ticketService = ticketService;
    }

    public ICommand ReturnCommand { get; set; }
    public ICommand QrCodeScannedCommand { get; set; }
    public ICommand ScanAgainCommand { get; set; }
    public ICommand ConsumeTicketCommand { get; set; }

    private void BindCommands()
    {
        ReturnCommand = new AsyncRelayCommand(GoBack);
        QrCodeScannedCommand = new AsyncRelayCommand<string>(QrCodeScannedAsync);
        ScanAgainCommand = new RelayCommand(ScanAgain);
        ConsumeTicketCommand = new AsyncRelayCommand(ConsumeTicketAsync);
    }

    private async Task GoBack()
    {
        await _navigationService.NavigateBackAsync();
    }

    public async Task CheckCameraPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Camera>();
        }

        if (status != PermissionStatus.Granted)
        {
            IsDetecting = false;
            StatusMessage = "Geef toestemming voor het gebruiken van de camera in de instellingen van uw smartphone.";
        }
    }

    private async Task QrCodeScannedAsync(string? value)
    {
        // The camera reports the same code many times per second: only handle the first one.
        if (!IsDetecting || string.IsNullOrWhiteSpace(value)) return;
        IsDetecting = false;

        StatusMessage = null;
        RequestResult = null;

        if (!Guid.TryParse(value, out Guid qrCode))
        {
            StatusMessage = "Deze QR-code is geen geldig ticket.";
            return;
        }

        IsBusy = true;
        try
        {
            RequestResult = await _ticketService.GetTicketByQrCode(qrCode);

            if (RequestResult.Used)
            {
                StatusMessage = "Dit ticket is al gebruikt.";
            }
            else if (!RequestResult.IsValidToday)
            {
                StatusMessage = "Dit ticket is vandaag niet geldig.";
            }
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

    private void ScanAgain()
    {
        StatusMessage = null;
        RequestResult = null;
        IsDetecting = true;
    }

    private async Task ConsumeTicketAsync()
    {
        if (RequestResult is null) return;

        IsBusy = true;
        try
        {
            await _ticketService.ConsumeTicket(RequestResult.Id);

            // Alles is succesvol afgerond, de conducteur kan naar de volgende passagier.
            ScanAgain();
            StatusMessage = "Ticket gebruikt. U kan de volgende passagier scannen.";
        }
        catch (TicketNotConsumableException)
        {
            StatusMessage = "Dit ticket kan niet gebruikt worden: het is al gebruikt of vandaag niet geldig.";
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
