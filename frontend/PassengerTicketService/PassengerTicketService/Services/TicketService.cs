using System.Net;
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

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new SessionExpiredException();
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadFromJsonAsync<PriceResult>()
               ?? throw new ApiException("Leeg antwoord van de server.");
    }

    public async Task<List<PassengerTicketResult>> GetMyTicketsAsync()
    {
        string requestUrl = "/api/ticket/passenger/me/all";

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadApiResultAsync<List<PassengerTicketResult>>();
    }

    public async Task<bool> PurchaseTicketsAsync(TravelRequest request)
    {
        // Converteer de DateTime naar DateOnly voor de backend
        var requestDto = new
        {
            StartStationId = request.StartStationId,
            EndStationId = request.EndStationId,
            PassengerCount = request.PassengerCount,
            IsRoundTrip = request.IsRoundTrip,
            ExpirationDate = DateOnly.FromDateTime(request.ExpirationDate),
            ReturnExpirationDate = DateOnly.FromDateTime(request.ReturnExpirationDate)
        };

        // De AuthHeaderHandler voegt automatisch het token toe!
        var response = await _httpClient.PostAsJsonAsync("api/ticket/purchase", requestDto);

        if (!response.IsSuccessStatusCode)
        {
            // Optioneel: lees de foutmelding uit de response (zoals "Station does not exist")
            var errorContent = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Purchase Error: {errorContent}");
        }

        return response.IsSuccessStatusCode;
    }
}
