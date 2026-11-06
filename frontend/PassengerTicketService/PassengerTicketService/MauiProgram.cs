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
        builder.Services.AddTransient<IStartViewModel, StartViewModel>();
        builder.Services.AddTransient<IRegisterViewModel, RegisterViewModel>();
        builder.Services.AddTransient<ILoginViewModel, LoginViewModel>();
        builder.Services.AddTransient<IProfileViewModel, ProfileViewModel>();
        builder.Services.AddTransient<IEditProfileViewModel, EditProfileViewModel>();

        // --- REGISTREER PAGES ---
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<YourTravelPage>();
        builder.Services.AddTransient<ShoppingCartPage>();
        builder.Services.AddTransient<TicketPage>();
        builder.Services.AddTransient<StartPage>();   
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<EditProfilePage>();

        return builder.Build();
    }
}