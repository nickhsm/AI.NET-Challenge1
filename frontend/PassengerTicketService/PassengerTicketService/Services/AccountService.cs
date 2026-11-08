using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using PassengerTicketService.Exceptions;
using PassengerTicketService.Models;

namespace PassengerTicketService.Services;

public class AccountService : IAccountService
{
    private readonly HttpClient _httpClient;

    public AccountService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PassengerProfileResult> GetProfileAsync()
    {
        string requestUrl = "/api/passenger/me";

        var response = await _httpClient.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }

        return await response.Content.ReadApiResultAsync<PassengerProfileResult>();
    }

    public async Task UpdateProfileAsync(string email, string? firstName, string? surname, DateOnly? dateOfBirth)
    {
        string requestUrl = "/api/passenger/me";

        // The property names match UpdateProfileRequestDto in the backend.
        var body = new { email, firstName, surname, dateOfBirth };
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
        string requestUrl = "/api/passenger/me/password";

        // The property names match ChangePasswordRequestDto in the backend.
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

    public async Task RegisterFaceAsync(Stream photo)
    {
        string requestUrl = "/api/passenger/me/face";

        using var photoContent = new StreamContent(photo);
        photoContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        // "photo" must match the parameter name of RegisterFace in the backend.
        using var form = new MultipartFormDataContent();
        form.Add(photoContent, "photo", "photo.jpg");

        var response = await _httpClient.PostAsync(requestUrl, form);

        if (response.StatusCode == HttpStatusCode.UnprocessableEntity)
        {
            throw new InvalidPhotoException();
        }

        if (response.StatusCode == HttpStatusCode.Conflict)
        {
            throw new FaceAlreadyRegisteredException();
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }
    }

    public async Task UnregisterFaceAsync()
    {
        string requestUrl = "/api/passenger/me/face";

        var response = await _httpClient.DeleteAsync(requestUrl);

        // 404: there is no face (anymore), which is what the passenger wanted.
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }
    }

    public async Task DeleteAccountAsync()
    {
        string requestUrl = "/api/passenger/me";

        var response = await _httpClient.DeleteAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
        {
            string message = await response.Content.ReadAsStringAsync();
            throw new ApiException(message);
        }
    }
}
