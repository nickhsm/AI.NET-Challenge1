using Microsoft.Extensions.Logging;
using PassengerTicketService.Services;
using PassengerTicketService.ViewModels;
using PassengerTicketService.Views;

namespace PassengerTicketService;

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

        // --- REGISTREER SERVICES ---
        builder.Services.AddSingleton<INavigationService, NavigationService>();

        // --- REGISTREER VIEWMODELS ---
        // Koppel de Interface aan de class
        builder.Services.AddTransient<IHomeViewModel, HomeViewModel>();
        builder.Services.AddTransient<IYourTravelViewModel, YourTravelViewModel>();
        // Voeg hier later ook de ShoppingCartViewModel en TicketsViewModel toe

        // --- REGISTREER PAGES ---
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<YourTravelPage>();
        builder.Services.AddTransient<TicketsPage>();

        return builder.Build();
    }
}