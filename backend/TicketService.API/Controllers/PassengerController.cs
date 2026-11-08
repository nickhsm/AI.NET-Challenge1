using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Domain.Entities;
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
    private readonly IPasswordHasher<IUser> _passwordHasher;

    public PassengerController(IFaceRecognitionService faceRecognitionService, IPassengerRepository passengerRepository,
        ITicketRepository ticketRepository, IPasswordHasher<IUser> passwordHasher)
    {
        _faceRecognitionService = faceRecognitionService;
        _passengerRepository = passengerRepository;
        _ticketRepository = ticketRepository;
        _passwordHasher = passwordHasher;
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
    
    [Authorize] // Heb nog geen rol restrictie gezet. Weet niet dat het ook gebruikt zal voor conducteur. 
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
    
    // --- NIEUWE ENDPOINT ---
    [Authorize]
    [HttpPut("me")]
    public async Task<ActionResult> UpdateMyProfile([FromBody] UpdateProfileRequestDto request)
    {
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Zoek de bestaande passagier
        var passenger = await _passengerRepository.GetPassengerByIdAsync(passengerId);
        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        // 3. Controleer of het nieuwe e-mailadres al door iemand anders wordt gebruikt
        if (passenger.Email.ToLower() != request.Email.ToLower())
        {
            var emailTaken = await _passengerRepository.EmailExistsAsync(request.Email);
            if (emailTaken)
            {
                return Conflict("Dit e-mailadres is al in gebruik door een ander account.");
            }
        }

        // 4. Werk de gegevens bij
        passenger.FirstName = request.FirstName;
        passenger.Surname = request.Surname;
        passenger.Email = request.Email;
        passenger.DateOfBirth = request.DateOfBirth;

        // 5. Sla de wijzigingen op
        await _passengerRepository.UpdatePassengerAsync(passenger);

        // 6. Geef een 204 No Content terug (geslaagd, geen data om terug te sturen)
        // Je zou er ook voor kunnen kiezen om de geüpdatete PassengerProfileDto terug te sturen.
        return NoContent();
    }
    
    // --- NIEUWE ENDPOINT ---
    [Authorize]
    [HttpPatch("me/password")]
    public async Task<ActionResult> ChangeMyPassword([FromBody] ChangePasswordRequestDto request)
    {
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Zoek de passagier
        var passenger = await _passengerRepository.GetPassengerByIdAsync(passengerId);
        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        // 3. Controleer of het HUIDIGE wachtwoord klopt met de Identity Hasher
        var verificationResult = _passwordHasher.VerifyHashedPassword(passenger, passenger.Password, request.CurrentPassword);
        
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return BadRequest("Het huidige wachtwoord is onjuist.");
        }

        // 4. Controleer of het nieuwe wachtwoord niet hetzelfde is als het oude
        if (request.CurrentPassword == request.NewPassword)
        {
            return BadRequest("Het nieuwe wachtwoord mag niet hetzelfde zijn als het huidige wachtwoord.");
        }

        // 5. Hash het NIEUWE wachtwoord met de Identity Hasher
        passenger.Password = _passwordHasher.HashPassword(passenger, request.NewPassword);

        // 6. Werk de passagier bij
        await _passengerRepository.UpdatePassengerAsync(passenger);

        // 7. Stuur een 204 No Content
        return NoContent();
    }
}