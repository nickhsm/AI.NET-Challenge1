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
        
        public async Task NavigateToShoppingCartPageAsync(TravelRequest request)
        {
            var page = _serviceProvider.GetRequiredService<ShoppingCartPage>();

            // Geef de request mee aan de pagina via de MAUI query dictionary
            var parameters = new Dictionary<string, object> { { "TravelRequest", request } };

            if (page.BindingContext is IQueryAttributable queryAttributable)
                queryAttributable.ApplyQueryAttributes(parameters);

            await Shell.Current.Navigation.PushAsync(page);
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

        public async Task NavigateToLoginPageAsync()
        {
            var page = _serviceProvider.GetRequiredService<LoginPage>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        public async Task NavigateToEditProfilePageAsync()
        {
            var page = _serviceProvider.GetRequiredService<EditProfilePage>();
            await Shell.Current.Navigation.PushAsync(page);
        }

        public async Task NavigateToStartPageAsync()
        {
            await Shell.Current.GoToAsync("//StartPage");
        }

        public async Task NavigateToHomeAfterLoginAsync()
        {
            await Shell.Current.Navigation.PopToRootAsync(false);
            await Shell.Current.GoToAsync("//HomePage");
        }

        public async Task NavigateToHomePageAsync()
        {
            if (Shell.Current.CurrentState.Location.OriginalString.Contains("HomePage")) return;
            await Shell.Current.GoToAsync("//HomePage");
        }

        public async Task NavigateToTicketsPageAsync()
        {
            if (Shell.Current.CurrentState.Location.OriginalString.Contains("TicketPage")) return;
            await Shell.Current.GoToAsync("//TicketPage");
        }

        public async Task NavigateToProfilePageAsync()
        {
            if (Shell.Current.CurrentState.Location.OriginalString.Contains("ProfilePage")) return;
            await Shell.Current.GoToAsync("//ProfilePage");
        }

        public async Task NavigateToChangePassword()
        {
            var page = _serviceProvider.GetRequiredService<ChangePasswordPage>();
            await Shell.Current.Navigation.PushAsync(page);
        }
    }
}