using System.Net.Http.Json;
using System.Text.Json;
using PassengerTicketService.Exceptions;

namespace PassengerTicketService.Services;

public static class HttpContentExtensions
{
    // ReadFromJsonAsync throws a JsonException when the body is not the JSON we expect,
    // for example when a field has a different type in the backend, or the body is an HTML page.
    // Turn it into an ApiException, because the ViewModels already handle that one.
    public static async Task<T> ReadApiResultAsync<T>(this HttpContent content)
    {
        T? result;
        try
        {
            result = await content.ReadFromJsonAsync<T>();
        }
        catch (JsonException ex)
        {
            throw new ApiException($"Antwoord kon niet gelezen worden: {ex.Message}");
        }

        // The body "null" is valid JSON and gives null instead of an exception,
        // but every endpoint we call should return a value.
        return result ?? throw new ApiException("Leeg antwoord van de server.");
    }
}
