using PassengerTicketService.Models;
using PassengerTicketService.Views;

namespace PassengerTicketService.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task NavigateToProfilePageAsync()
        {
            throw new NotImplementedException();
            //await Shell.Current.GoToAsync("//ProfilePage");
        }

        public async Task NavigateBackAsync()
        {
            // Gebruik Shell.Current direct, dit voorkomt de null-waarschuwing in de constructor
            if (Shell.Current.Navigation.NavigationStack.Count > 1)
            {
                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                throw new InvalidOperationException("No pages to navigate back to!");
            }
        }
        
        public async Task NavigateToShoppingCartPageAsync(TravelRequest request)
        {
            var page = _serviceProvider.GetRequiredService<ShoppingCartPage>();

            // Geef de request mee aan de pagina via de MAUI query dictionary
            var parameters = new Dictionary<string, object> { { "TravelRequest", request } };

            if (page.BindingContext is IQueryAttributable queryAttributable)
                queryAttributable.ApplyQueryAttributes(parameters);

            await Shell.Current.Navigation.PushAsync(page);
        }

        // Navbar 'Tickets'-button: haal de TicketPage uit de DI-container.
        // NB: because AddTransient gebuild a new page object (with a new ViewModel) per push,
        // de ViewModel always gets de BindingContext of a new ViewModel.
        public async Task NavigateToTicketsPageAsync()
        {
            await Shell.Current.GoToAsync("//TicketPage");
        }

        // Navbar 'Home'-button: HomePage heeft geen data te pass along, dus no
        // ApplyQueryAttributes-call is needed.
        public async Task NavigateToHomePageAsync()
        {
            // De dubbele slash (//) zorgt ervoor dat de stack wordt gereset
            await Shell.Current.GoToAsync("//HomePage");
        }

        // FIX: Gecorrigeerd naar IQueryAttributable in plaats van concrete ViewModel
        public async Task NavigateToYourTravelPageAsync(TravelRequest request)
        {
            // 1. Haal de pagina op uit de DI container
            var page = _serviceProvider.GetRequiredService<YourTravelPage>();

            // 2. Geef de data door via de standaard MAUI dictionary
            var parameters = new Dictionary<string, object> { { "TravelRequest", request } };

            // 3. Kijk of de pagina de data kan ontvangen (via IQueryAttributable)
            // Kijk naar de BindingContext (het ViewModel) in plaats van de Page
            if (page.BindingContext is IQueryAttributable queryAttributable)
                queryAttributable.ApplyQueryAttributes(parameters);

            // 4. Navigeer naar de pagina
            await Shell.Current.Navigation.PushAsync(page);
        }

        public async Task NavigateToRegisterPageAsync()
        {
            var page = _serviceProvider.GetRequiredService<RegisterPage>();
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
}