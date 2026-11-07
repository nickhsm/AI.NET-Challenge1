using ConductorTicketService.Services;
using ConductorTicketService.ViewModels;
using ConductorTicketService.Views;
using System.Reflection;
using Microsoft.Extensions.Logging;
using ZXing.Net.Maui.Controls;

namespace ConductorTicketService;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // HttpClient for requests to endpoint.
        // The url comes from frontend/Directory.Build.props (or your own ApiBaseUrl.local.props).
        Uri baseAddress = new Uri(GetApiBaseUrl());

        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = baseAddress;
        });

        builder.Services.AddTransient<AuthHeaderHandler>();
        builder.Services.AddHttpClient<IFaceCheckService, FaceCheckService>(client =>
        {
            client.BaseAddress = baseAddress;
        }).AddHttpMessageHandler<AuthHeaderHandler>();
        builder.Services.AddHttpClient<ITicketService, TicketService>(client =>
        {
            client.BaseAddress = baseAddress;
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();

        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ILoginViewModel, LoginViewModel>();

        builder.Services.AddTransient<ScanListPage>();
        builder.Services.AddTransient<IScanListViewModel, ScanListViewModel>();

        builder.Services.AddTransient<QrCodePage>();
        builder.Services.AddTransient<IQrCodeViewModel, QrCodeViewModel>();

        builder.Services.AddTransient<FacePage>();
        builder.Services.AddTransient<IFaceViewModel, FaceViewModel>();

        return builder.Build();
    }

    // Reads the url that was baked into the app at build time from Directory.Build.props.
    private static string GetApiBaseUrl()
    {
        return typeof(MauiProgram).Assembly
                   .GetCustomAttributes<AssemblyMetadataAttribute>()
                   .FirstOrDefault(attribute => attribute.Key == "ApiBaseUrl")?.Value
               ?? throw new InvalidOperationException("ApiBaseUrl is missing, check frontend/Directory.Build.props.");
    }
}