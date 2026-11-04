using Microsoft.Extensions.DependencyInjection;

namespace PassengerTicketService;

public partial class App : Application
{
    public App()
    {
        AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
        {
            System.Diagnostics.Debug.WriteLine($"FIRST CHANCE EXCEPTION: {eventArgs.Exception}");
        };

        AppDomain.CurrentDomain.UnhandledException += (sender, eventArgs) =>
        {
            System.Diagnostics.Debug.WriteLine($"UNHANDLED EXCEPTION: {eventArgs.ExceptionObject}");
        };

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}