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
}