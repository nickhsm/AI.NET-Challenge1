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

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<ScanListPage>();
        builder.Services.AddTransient<IScanListViewModel, ScanListViewModel>();

        builder.Services.AddTransient<QrCodePage>();
        builder.Services.AddTransient<IQrCodeViewModel, QrCodeViewModel>();

        return builder.Build();
    }
}