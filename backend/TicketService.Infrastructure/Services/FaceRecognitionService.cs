using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using TicketService.Infrastructure.Exceptions;

namespace TicketService.Infrastructure.Services;

public class FaceRecognitionService : IFaceRecognitionService
{
    // Configuration of HttpClient happens in program.cs
    private readonly HttpClient _httpClient;
    private readonly string _groupId;


    public FaceRecognitionService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _groupId = configuration["Face:GroupId"]
                   ?? throw new InvalidOperationException("Face:GroupId is missing in user secrets.");
    }

    // Helper method the retrain
    private async Task RetrainAsync()
    {
        // Step 1
        string startTrainingUrl = $"/face/v1.2/largepersongroups/{_groupId}/train";

        var responseStartTraining = await _httpClient.PostAsync(startTrainingUrl, null);

        // Should always respond with 202
        if (!responseStartTraining.IsSuccessStatusCode)
        {
            var error = await responseStartTraining.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }
        // Step 1 end

        // Step 2
        string checkTrainingUrl = $"/face/v1.2/largepersongroups/{_groupId}/training";

        string? status;
        do
        {
            // Training will take longer than this with a **very** large dataset.
            await Task.Delay(2000);

            var responseCheckTraining = await _httpClient.GetAsync(checkTrainingUrl);

            // Should always respond with 200
            if (!responseCheckTraining.IsSuccessStatusCode)
            {
                var error = await responseCheckTraining.Content.ReadAsStringAsync();
                throw new AzureServiceException(error);
            }

            string json = await responseCheckTraining.Content.ReadAsStringAsync();
            using var parsedJson = JsonDocument.Parse(json);

            status = parsedJson.RootElement.GetProperty("status").GetString();

            if (status == "succeeded")
            {
                return;
            }

            if (status == "failed")
            {
                throw new AzureServiceException(json);
            }
        } while (status is "running" or "notStarted");
        // Step 2 end
    }

    // Azure errors look like {"error":{"code":"...","message":"..."}}.
    // Returns null when the body is not in that shape.
    private static string? GetAzureErrorCode(string errorJson)
    {
        try
        {
            using var parsedJson = JsonDocument.Parse(errorJson);
            if (parsedJson.RootElement.TryGetProperty("error", out var error) &&
                error.TryGetProperty("code", out var code))
            {
                return code.GetString();
            }
        }
        catch (JsonException)
        {
            // Not JSON, so there is no error code to read.
        }

        return null;
    }

    private async Task DeletePersonAsync(Guid personId)
    {
        string deletePersonUrl = $"/face/v1.2/largepersongroups/{_groupId}/persons/{personId}";

        var responseDeletePerson = await _httpClient.DeleteAsync(deletePersonUrl);

        // We can continue because there is nothing to do.
        if (responseDeletePerson.StatusCode == HttpStatusCode.NotFound)
        {
            return;
        }

        if (!responseDeletePerson.IsSuccessStatusCode)
        {
            var error = await responseDeletePerson.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }
    }

    public async Task<Guid> EnrollAsync(Stream photo)
    {
        // Step 1
        // this gets added after the base url. This has been set in program.cs
        string registerPersonUrl = $"/face/v1.2/largepersongroups/{_groupId}/persons";

        // The name parameter can be anything random. We will not need this any more. We are interested in the response.
        var responseRegisterPerson =
            await _httpClient.PostAsJsonAsync(registerPersonUrl, new { name = Guid.NewGuid().ToString() });

        if (!responseRegisterPerson.IsSuccessStatusCode)
        {
            var error = await responseRegisterPerson.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }

        string json = await responseRegisterPerson.Content.ReadAsStringAsync();
        using var parsedJson = JsonDocument.Parse(json);

        Guid personId = parsedJson.RootElement.GetProperty("personId").GetGuid();
        // Step 1 end

        // Step 2
        try
        {
            string addImageToPersonUrl =
                $"/face/v1.2/largepersongroups/{_groupId}/persons/{personId}/persistedfaces?detectionModel=detection_03";
            using var content = new StreamContent(photo);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var responseAddImageToPerson = await _httpClient.PostAsync(addImageToPersonUrl, content);

            if (!responseAddImageToPerson.IsSuccessStatusCode)
            {
                var error = await responseAddImageToPerson.Content.ReadAsStringAsync();
                throw new AzureServiceException(error);
            }

            // I don't need the content of the response. If it is successful, that means everything is OK.
            await RetrainAsync();
        }
        catch
        {
            // Making sure that the person is deleted if the image did not work.
            await DeletePersonAsync(personId);

            // `catch` catches the exception in the try block.
            // Then this `throw` here below, throws it for real. Then it is someone else's problem.
            throw;
        }
        // Step 2 end

        return personId;
    }

    public async Task<Guid?> IdentifyAsync(Stream photo)
    {
        // Step 1
        string detectFaceUrl =
            "/face/v1.2/detect?detectionModel=detection_03&recognitionModel=recognition_04&returnFaceId=true";
        using var contentDetectFace = new StreamContent(photo);
        contentDetectFace.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

        var responseDetect = await _httpClient.PostAsync(detectFaceUrl, contentDetectFace);

        if (!responseDetect.IsSuccessStatusCode)
        {
            var error = await responseDetect.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }

        string jsonDetect = await responseDetect.Content.ReadAsStringAsync();
        using var parsedJsonDetect = JsonDocument.Parse(jsonDetect);

        if (parsedJsonDetect.RootElement.GetArrayLength() == 0)
        {
            throw new InvalidFaceCountException("No face detected");
        }

        if (parsedJsonDetect.RootElement.GetArrayLength() > 1)
        {
            throw new InvalidFaceCountException("Too many faces detected");
        }

        // We know that faceId cannot be null, because there is already an error check.
        Guid faceId = parsedJsonDetect.RootElement[0].GetProperty("faceId").GetGuid();
        // Step 1 end

        // Step 2
        string identifyFaceUrl = "/face/v1.2/identify";

        var responseIdentifyFace = await _httpClient.PostAsJsonAsync(identifyFaceUrl,
            new
            {
                faceIds = new[] { faceId },
                largePersonGroupId = _groupId,
                maxNumOfCandidatesReturned = 1
            });

        if (!responseIdentifyFace.IsSuccessStatusCode)
        {
            var error = await responseIdentifyFace.Content.ReadAsStringAsync();

            // A group that was never trained (e.g. right after it was recreated) has no one to match against.
            // That is "no match", not an Azure failure.
            if (GetAzureErrorCode(error) == "LargePersonGroupNotTrained")
            {
                return null;
            }

            throw new AzureServiceException(error);
        }

        string jsonIdentify = await responseIdentifyFace.Content.ReadAsStringAsync();
        using var parsedJsonIdentify = JsonDocument.Parse(jsonIdentify);

        // This element should always be available
        var candidates = parsedJsonIdentify.RootElement[0].GetProperty("candidates");

        if (candidates.GetArrayLength() == 0)
        {
            return null;
        }

        Guid personId = candidates[0].GetProperty("personId").GetGuid();
        // Step 2 end

        return personId;
    }

    public async Task DeleteAsync(Guid personId)
    {
        await DeletePersonAsync(personId);

        // Azure cannot train a group without faces.
        // If that was the last person, reset the group instead: an empty, untrained group
        // makes IdentifyAsync return "no match", which is exactly right.
        if (!await GroupHasPersonsAsync())
        {
            await DeleteAllAsync();
            return;
        }

        await RetrainAsync();

        // If successful, person is deleted.
    }

    private async Task<bool> GroupHasPersonsAsync()
    {
        // top=1: we only need to know if there is at least one person.
        string listPersonsUrl = $"/face/v1.2/largepersongroups/{_groupId}/persons?top=1";

        var responseListPersons = await _httpClient.GetAsync(listPersonsUrl);

        if (!responseListPersons.IsSuccessStatusCode)
        {
            var error = await responseListPersons.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }

        string json = await responseListPersons.Content.ReadAsStringAsync();
        using var parsedJson = JsonDocument.Parse(json);

        return parsedJson.RootElement.GetArrayLength() > 0;
    }

    public async Task DeleteAllAsync()
    {
        // Deleting every person one by one would leave an empty group behind,
        // and Azure cannot train a group without persons.
        // So we delete the whole group and create a fresh, empty one with the same id.
        string groupUrl = $"/face/v1.2/largepersongroups/{_groupId}";

        // Step 1: delete the group, including all persons and faces in it.
        var responseDeleteGroup = await _httpClient.DeleteAsync(groupUrl);

        // 404 means the group was already gone, so there is nothing to delete.
        if (!responseDeleteGroup.IsSuccessStatusCode && responseDeleteGroup.StatusCode != HttpStatusCode.NotFound)
        {
            var error = await responseDeleteGroup.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }

        // Step 2: create the group again.
        // recognitionModel must match the one used in IdentifyAsync.
        var responseCreateGroup = await _httpClient.PutAsJsonAsync(groupUrl,
            new
            {
                name = _groupId,
                recognitionModel = "recognition_04"
            });

        if (!responseCreateGroup.IsSuccessStatusCode)
        {
            var error = await responseCreateGroup.Content.ReadAsStringAsync();
            throw new AzureServiceException(error);
        }
    }
}