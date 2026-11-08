using PassengerTicketService.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;      // <-- Nodig voor HttpClient
using System.Net.Http.Json;

namespace PassengerTicketService.Services
{
    public class ApiService: IApiService
    {
        // 1. Declareer het private veld
        private readonly HttpClient _httpClient;

        // 2. Injecteer de HttpClient via de constructor
        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<StationDto>> SearchStationsAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<StationDto>();

            // De AuthHeaderHandler voegt automatisch het Bearer token toe!
            var result = await _httpClient.GetFromJsonAsync<IEnumerable<StationDto>>($"api/stations?searchQuery={Uri.EscapeDataString(query)}");
            return result ?? Enumerable.Empty<StationDto>();
        }
    }
}
