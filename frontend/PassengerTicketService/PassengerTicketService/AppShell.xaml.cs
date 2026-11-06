using PassengerTicketService.Views;

namespace PassengerTicketService;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("HomePage", typeof(Views.HomePage));
        Routing.RegisterRoute("TicketPage", typeof(Views.TicketPage));
        Routing.RegisterRoute("StartPage", typeof(Views.StartPage));
    }
}