using Microsoft.Extensions.DependencyInjection;

namespace PassengerTicketService;

public partial class App : Application
{
    public App()
    {
#if DEBUG
        // Deze code wordt ALLEEN uitgevoerd als je de app in 'Debug' modus bouwt.
        // Het is handig tijdens het ontwikkelen, maar kan in 'Release' voor overhead zorgen.
    
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            System.Diagnostics.Debug.WriteLine($"FIRST CHANCE EXCEPTION: {eventArgs.Exception}");
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
        {
            System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {eventArgs.ExceptionObject}");
        };
#endif
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}