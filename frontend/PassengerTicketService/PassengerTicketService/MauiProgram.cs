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
        builder.Services.AddTransient<IShoppingCartViewModel, ShoppingCartViewModel>();
        builder.Services.AddTransient<ITicketViewModel, TicketViewModel>();

        // --- REGISTREER PAGES ---
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<YourTravelPage>();
        builder.Services.AddTransient<ShoppingCartPage>();
        builder.Services.AddTransient<TicketPage>();

        return builder.Build();
    }
}