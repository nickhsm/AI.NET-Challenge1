using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public class AuthService : IAuthService
{
    private const string TokenKey = "auth_token";
    private const string ExpiresAtKey = "auth_expires_at";

    private HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task LoginAsync(string email, string password)
    {
        string requestUrl = "/api/auth/conductor/login";

        var response = await _httpClient.PostAsJsonAsync(requestUrl, new { email, password });

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new InvalidLoginException();
        }

        await SaveTokenAsync(response);
    }

    public async Task RegisterAsync(string email, string password, string? firstName, string? surname, string registrationCode)
    {
        string requestUrl = "/api/auth/conductor/register";

        // The property names match ConductorRegisterDto in the backend.
        // No birth date: the backend doesn't require it and the conductor app doesn't use it.
        var body = new { email, password, firstName, surname, registrationCode };
        var response = await _httpClient.PostAsJsonAsync(requestUrl, body);

        if (response.StatusCode == HttpStatusCode.Forbidden)
        {
            throw new InvalidRegistrationCodeException();
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new EmailAlreadyInUseException();
        }

        // Register returns the same token as login, so the conductor is logged in straight away.
        await SaveTokenAsync(response);
    }

    // Shared by login and register: both answer with a LoginResult on success.
    private async Task SaveTokenAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        var result = await response.Content.ReadFromJsonAsync<LoginResult>() ??
                     throw new ApiException("Empty response body.");

        await SecureStorage.Default.SetAsync(TokenKey, result.Token);
        // "o" saves the date in a fixed format (ISO 8601) that can be read back exactly.
        await SecureStorage.Default.SetAsync(ExpiresAtKey, result.ExpiresAt.ToString("o"));
    }

    public async Task<bool> IsLoggedInAsync()
    {
        return await GetTokenAsync() is not null;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            string? token = await SecureStorage.Default.GetAsync(TokenKey);
            string? expiresAt = await SecureStorage.Default.GetAsync(ExpiresAtKey);

            if (token is null || expiresAt is null)
            {
                return null;
            }

            DateTime expires = DateTime.Parse(expiresAt, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            // Token expired
            if (expires <= DateTime.UtcNow)
            {
                Logout();
                return null;
            }

            return token;
        }
        catch (Exception)
        {
            // SecureStorage can fail on Android, for example after a reinstall.
            // The saved values are unreadable then, so start over.
            Logout();
            return null;
        }
    }

    public void Logout()
    {
        SecureStorage.Default.Remove(TokenKey);
        SecureStorage.Default.Remove(ExpiresAtKey);
    }
}