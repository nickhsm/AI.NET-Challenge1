using ConductorTicketService.ViewModels;
using ZXing.Net.Maui;

namespace ConductorTicketService.Views;

public partial class QrCodePage : ContentPage
{
    private readonly IQrCodeViewModel _viewModel;

    public QrCodePage(IQrCodeViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;

        BarcodeReader.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormat.QrCode,
            AutoRotate = true,
            Multiple = false
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.CheckCameraPermissionAsync();
    }

    // The camera view only exposes an event, so forward it to the view model's command.
    // The event fires on a background thread: switch to the UI thread before touching bound properties.
    private void OnBarcodesDetected(object? sender, BarcodeDetectionEventArgs e)
    {
        string? value = e.Results.FirstOrDefault()?.Value;

        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (_viewModel.QrCodeScannedCommand.CanExecute(value))
            {
                _viewModel.QrCodeScannedCommand.Execute(value);
            }
        });
    }
}
