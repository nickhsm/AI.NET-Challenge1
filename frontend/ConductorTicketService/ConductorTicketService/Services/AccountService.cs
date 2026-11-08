using System.Net;
using System.Net.Http.Json;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ConductorProfileResult> GetProfileAsync()
    {
        string requestUrl = "/api/conductor/me";

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadApiResultAsync<ConductorProfileResult>();
    }

    public async Task UpdateProfileAsync(string email, string? firstName, string? surname)
    {
        string requestUrl = "/api/conductor/me";

        // The property names match UpdateConductorProfileDto in the backend.
        var body = new { email, firstName, surname };
        var response = await _httpClient.PutAsJsonAsync(requestUrl, body);

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new EmailAlreadyInUseException();
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }
    }

    public async Task ChangePasswordAsync(string currentPassword, string newPassword)
    {
        string requestUrl = "/api/conductor/me/password";

        // The property names match ConductorChangePasswordDto in the backend.
        var body = new { currentPassword, newPassword };
        var response = await _httpClient.PatchAsJsonAsync(requestUrl, body);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new WrongPasswordException();
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }
    }
}
