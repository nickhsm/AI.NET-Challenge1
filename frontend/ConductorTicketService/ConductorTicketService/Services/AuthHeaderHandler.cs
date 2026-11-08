using System.Net;
using System.Net.Http.Headers;

namespace ConductorTicketService.Services;

// Runs before every request of the HttpClients it is added to (see MauiProgram.cs),
// and adds the token as "Authorization: Bearer <token>".
public class AuthHeaderHandler : DelegatingHandler
{
    private readonly IAuthService _authService;
    private readonly INavigationService _navigationService;

    public AuthHeaderHandler(IAuthService authService, INavigationService navigationService)
    {
        _authService = authService;
        _navigationService = navigationService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await _authService.GetTokenAsync();
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        var response = await base.SendAsync(request, cancellationToken);

        // The backend rejected the token (expired, or the server key changed).
        // Throw it away and send the user back to the login page.
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            _authService.Logout();
            await _navigationService.NavigateToLoginPageAsync();
        }

        return response;
    }
}
