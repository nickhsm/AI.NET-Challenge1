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