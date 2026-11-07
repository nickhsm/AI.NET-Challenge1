using System.Net.Http.Headers;

namespace ConductorTicketService.Services;

public class AuthHeaderHandler : DelegatingHandler
{
    private IAuthService _authService;

    public AuthHeaderHandler(IAuthService authService)
    {
        _authService = authService;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        string? token = await _authService.GetTokenAsync();
        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}