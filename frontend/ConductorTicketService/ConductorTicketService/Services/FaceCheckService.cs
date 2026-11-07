using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using ConductorTicketService.Exceptions;
using ConductorTicketService.Models;

namespace ConductorTicketService.Services;

public class FaceCheckService : IFaceCheckService
{
    private readonly HttpClient _httpClient;

    public FaceCheckService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CheckFaceResult?> CheckFaceAsync(Stream photo)
    {
        string endpointString = "/api/passenger/check-face";

        // Van Stream naar StreamContent
        using var photoContent = new StreamContent(photo);
        photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        // StreamContent naar een multipart form data
        using var form = new MultipartFormDataContent();
        form.Add(photoContent, "photo", "photo.jpg");

        var response = await _httpClient.PostAsync(endpointString, form);

        if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new InvalidPhotoException(message);
        }

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadFromJsonAsync<CheckFaceResult>();
    }
}