using PassengerTicketService.Views;

namespace PassengerTicketService.Services
{
    public class NavigationService : INavigationService
    {
        private INavigation _navigation;
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _navigation = Shell.Current?.Navigation;
        }

        public async Task NavigateBackAsync()
        {
            if (_navigation.NavigationStack.Count > 1)
            {
                await _navigation.PopAsync();
            }
            else
            {
                throw new InvalidOperationException("No pages to navigate back to!");
            }
        }

        public Task NavigateToShoppingCartPageAsync()
        {
            throw new NotImplementedException();
        }

        public Task NavigateToTicketsPageAsync()
        {
            throw new NotImplementedException();
        }

        public async Task NavigateToYourTravelPageAsync()
        {
            await _navigation.PushAsync(_serviceProvider.GetRequiredService<YourTravelPage>());
        }
    }
}
