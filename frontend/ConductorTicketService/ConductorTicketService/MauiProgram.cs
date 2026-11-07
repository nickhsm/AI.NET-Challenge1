using ConductorTicketService.Services;
using ConductorTicketService.ViewModels;
using ConductorTicketService.Views;
using Microsoft.Extensions.Logging;

namespace ConductorTicketService;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // HttpClient for requests to endpoint.
        builder.Services.AddHttpClient<IFaceCheckService, FaceCheckService>(client =>
        {
            client.BaseAddress = new Uri("http://10.10.1.188:5271");
        });

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<ScanListPage>();
        builder.Services.AddTransient<IScanListViewModel, ScanListViewModel>();

        builder.Services.AddTransient<QrCodePage>();
        builder.Services.AddTransient<IQrCodeViewModel, QrCodeViewModel>();

        builder.Services.AddTransient<FacePage>();
        builder.Services.AddTransient<IFaceViewModel, FaceViewModel>();

        return builder.Build();
    }
}