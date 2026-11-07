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

        // --- HTTPCLIENTS VOOR DE BACKEND ---
        Uri baseAddress = new Uri("http://10.10.1.188:5271");

        // Login does not need a token, so no AuthHeaderHandler here.
        builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = baseAddress;
        });

        // Adds the token to every request of the clients it is attached to
        // with .AddHttpMessageHandler<AuthHeaderHandler>().
        builder.Services.AddTransient<AuthHeaderHandler>();

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
        builder.Services.AddTransient<IChangePasswordViewModel, ChangePasswordViewModel>();
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
        builder.Services.AddTransient<ChangePasswordPage>();
        builder.Services.AddTransient<EditProfilePage>();

        return builder.Build();
    }
}