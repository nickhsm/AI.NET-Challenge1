using System.Net;
using System.Net.Http.Json;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public class TicketService : ITicketService
{
    private HttpClient _httpClient;

    public TicketService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TicketListItemResult>> GetTicketsByFacePersonId(string facePersonId)
    {
        string requestUrl = $"/api/ticket/passenger/{facePersonId}";

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new PassengerNotFoundException(message);
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        // If list is empty return empty list.
        return await response.Content.ReadFromJsonAsync<List<TicketListItemResult>>() ?? [];
    }

    public async Task ConsumeTicket(int ticketId)
    {
        string requestUrl = $"/api/ticket/consume/{ticketId}";

        var response = await _httpClient.PatchAsync(requestUrl, null);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new TicketNotFoundException(message);
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new TicketNotConsumableException(message);
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

    }

    public async Task<TicketQrCodeResult> GetTicketByQrCode(Guid qrCode)
    {
        string requestUrl = $"/api/ticket/qr/{qrCode}";

        var response = await _httpClient.GetAsync(requestUrl);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new TicketNotFoundException(message);
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadFromJsonAsync<TicketQrCodeResult>()
               ?? throw new ApiException("Empty response body.");
    }
}