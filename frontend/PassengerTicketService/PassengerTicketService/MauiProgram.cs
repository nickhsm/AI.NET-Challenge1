using Microsoft.Extensions.Logging;
using PassengerTicketService.Services;
using PassengerTicketService.ViewModels;
using PassengerTicketService.Views;
using System.Reflection;

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
        // De url komt uit frontend/Directory.Build.props (of je eigen ApiBaseUrl.local.props).
        Uri baseAddress = new Uri(GetApiBaseUrl());

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

    // Leest de url die tijdens het builden uit Directory.Build.props in de app is gezet.
    private static string GetApiBaseUrl()
    {
        return typeof(MauiProgram).Assembly
                   .GetCustomAttributes<AssemblyMetadataAttribute>()
                   .FirstOrDefault(attribute => attribute.Key == "ApiBaseUrl")?.Value
               ?? throw new InvalidOperationException("ApiBaseUrl is missing, check frontend/Directory.Build.props.");
    }
}