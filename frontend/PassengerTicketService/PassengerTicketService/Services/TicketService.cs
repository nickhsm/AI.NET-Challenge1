using System.Net.Http.Json;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Models;

namespace PassengerTicketService.Services;

public class TicketService : ITicketService
{
    private readonly HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PriceResult> CalculatePriceAsync(int passengerCount, bool isRoundTrip)
    {
        string requestUrl = "/api/ticket/calculate-price";

        // The property names match PriceCalculationRequestDto in the backend.
        var body = new { passengerCount, isRoundTrip };
        var response = await _httpClient.PostAsJsonAsync(requestUrl, body);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadFromJsonAsync<PriceResult>()
               ?? throw new ApiException("Leeg antwoord van de server.");
    }
}
