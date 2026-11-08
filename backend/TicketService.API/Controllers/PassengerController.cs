using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Services;
using TicketService.Shared;

namespace TicketService.API.Controllers;

[Route("api/passenger")]
[ApiController]
public class PassengerController : ControllerBase
{
    private IFaceRecognitionService _faceRecognitionService;
    private IPassengerRepository _passengerRepository;
    private ITicketRepository _ticketRepository;

    public PassengerController(IFaceRecognitionService faceRecognitionService, IPassengerRepository passengerRepository,
        ITicketRepository ticketRepository)
    {
        _faceRecognitionService = faceRecognitionService;
        _passengerRepository = passengerRepository;
        _ticketRepository = ticketRepository;
    }

    private ActionResult? ValidatePhoto(IFormFile photo)
    {
        string[] allowedImageTypes = ["image/jpeg", "image/png"];

        if (!allowedImageTypes.Contains(photo.ContentType))
        {
            return StatusCode(StatusCodes.Status415UnsupportedMediaType, "The file must be a jpg or png image.");
        }

        if (photo.Length > 6 * 1024 * 1024)
        {
            return StatusCode(StatusCodes.Status413PayloadTooLarge, "The file must be less than 6 MB.");
        }

        return null;
    }

    private int GetPassengerId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [Authorize(Roles = Roles.Passenger)]
    [HttpPost("me/face")]
    public async Task<ActionResult<PassengerFaceDto>> RegisterFace(IFormFile photo)
    {
        int id = GetPassengerId();

        ActionResult? photoError = ValidatePhoto(photo);
        if (photoError is not null)
        {
            return photoError;
        }

        var passenger = await _passengerRepository.GetPassengerByIdAsync(id);

        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        if (passenger.FacePersonId is not null)
        {
            return Conflict("This passenger already has a registered face.");
        }

        // 1 person cannot register his face in more than 1 account.
        try
        {
            await using var imageStream = photo.OpenReadStream();
            Guid? existingPersonId = await _faceRecognitionService.IdentifyAsync(imageStream);

            if (existingPersonId is not null)
            {
                return Conflict("This face is already registered on another account.");
            }
        }
        catch (InvalidFaceCountException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
        catch (AzureServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }

        Guid personId;
        try
        {
            await using var imageStream = photo.OpenReadStream();
            personId = await _faceRecognitionService.EnrollAsync(imageStream);
        }
        catch (AzureServiceException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, exception.Message);
        }

        _passengerRepository.RegisterFace(id, personId);

        PassengerFaceDto mappedResult = PassengerMapper.ToPassengerFaceDto(personId.ToString());

        return StatusCode(StatusCodes.Status201Created, mappedResult);
    }

    [Authorize(Roles = Roles.Passenger)]
    [HttpDelete("me/face")]
    public async Task<ActionResult> UnregisterFace()
    {
        int id = GetPassengerId();

        var passenger = await _passengerRepository.GetPassengerByIdAsync(id);
        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        if (passenger.FacePersonId is null)
        {
            return NotFound("Passenger does not have a face registered.");
        }

        try
        {
            await _faceRecognitionService.DeleteAsync(passenger.FacePersonId.Value);
        }
        catch (AzureServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }

        await _passengerRepository.UnregisterFaceAsync(id);

        return NoContent();
    }

    [Authorize(Roles = Roles.Conductor)]
    [HttpPost("check-face")]
    public async Task<ActionResult<CheckFaceDto>> CheckFace(IFormFile photo)
    {
        ActionResult? photoError = ValidatePhoto(photo);
        if (photoError is not null)
        {
            return photoError;
        }

        // Needs refactor because the conductor cannot know which face a PassengerFaceId is.
        // We need to be able to send an image with the user.
        // So he can see the image and compare with the person standing in from of him.
        await using var imageStream = photo.OpenReadStream();

        Guid? facePersonId;
        try
        {
            facePersonId = await _faceRecognitionService.IdentifyAsync(imageStream);
        }
        catch (AzureServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        catch (InvalidFaceCountException ex)
        {
            return UnprocessableEntity(ex.Message);
        }

        if (facePersonId is null)
        {
            return NotFound("Face does not exist.");
        }

        var passenger = await _passengerRepository.GetPassengerByFacePersonIdAsync(facePersonId.Value);

        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        bool hasTickets = await _ticketRepository.PassengerHasValidTicketAsync(facePersonId.Value);

        CheckFaceDto mappedResponse = PassengerMapper.ToCheckFaceDto(passenger, hasTickets);

        return Ok(mappedResponse);
    }
    
    [Authorize (Roles=Roles.Passenger)] 
    [HttpGet("me")]
    public async Task<ActionResult<PassengerProfileDto>> GetMyProfile()
    {
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Zoek de passagier in de database
        var passenger = await _passengerRepository.GetPassengerByIdAsync(passengerId);

        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        // 3. Map naar de DTO (inclusief de check of er een FacePersonId is)
        var mappedProfile = passenger.ToPassengerProfileDto();

        return Ok(mappedProfile);
    }
    
    [Authorize] // Heb nog geen rol restrictie gezet. Weet niet dat het ook gebruikt zal voor conducteur.
    [HttpDelete("me")]
    public async Task<ActionResult> DeleteMyAccount()
    {
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Probeer de passagier te verwijderen
        bool isDeleted = await _passengerRepository.DeletePassengerAsync(passengerId);

        if (!isDeleted)
        {
            return NotFound("Passenger does not exist.");
        }

        // 3. Geef een 204 No Content terug (standaard voor een succesvolle DELETE)
        return NoContent();
    }
}