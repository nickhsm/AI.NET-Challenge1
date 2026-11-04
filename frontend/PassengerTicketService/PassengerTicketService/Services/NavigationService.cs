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

        // FIX: 'async' keyword toegevoegd
        public async Task NavigateToShoppingCartPageAsync(TravelRequest request)
        {
            var page = _serviceProvider.GetRequiredService<ShoppingCartPage>();

            // Geef de request mee aan de pagina via de MAUI query dictionary
            var parameters = new Dictionary<string, object> { { "TravelRequest", request } };

            if (page is IQueryAttributable queryAttributable)
                queryAttributable.ApplyQueryAttributes(parameters);

            await Shell.Current.Navigation.PushAsync(page);
        }

        public Task NavigateToTicketsPageAsync()
        {
            throw new NotImplementedException();
        }

        // FIX: Gecorrigeerd naar IQueryAttributable in plaats van concrete ViewModel
        public async Task NavigateToYourTravelPageAsync(TravelRequest request)
        {
            // 1. Haal de pagina op uit de DI container
            var page = _serviceProvider.GetRequiredService<YourTravelPage>();

            // 2. Geef de data door via de standaard MAUI dictionary
            var parameters = new Dictionary<string, object> { { "TravelRequest", request } };

            // 3. Kijk of de pagina de data kan ontvangen (via IQueryAttributable)
            if (page is IQueryAttributable queryAttributable)
                queryAttributable.ApplyQueryAttributes(parameters);

            // 4. Navigeer naar de pagina
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
}