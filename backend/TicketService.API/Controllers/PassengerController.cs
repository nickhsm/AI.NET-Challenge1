using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Services;

namespace TicketService.API.Controllers;

[Route("api/passenger")]
[ApiController]
public class PassengerController : ControllerBase
{
    private IFaceRecognitionService _faceRecognitionService;
    private IPassengerRepository _passengerRepository;

    public PassengerController(IFaceRecognitionService faceRecognitionService, IPassengerRepository passengerRepository)
    {
        _faceRecognitionService = faceRecognitionService;
        _passengerRepository = passengerRepository;
    }

    [HttpPost("{id}/face")]
    public async Task<ActionResult<PassengerFaceDto>> RegisterFace(int id,
        IFormFile photo)
    {
        string[] allowedImageTypes = ["image/jpeg", "image/png"];

        if (!allowedImageTypes.Contains(photo.ContentType))
        {
            return BadRequest("The file must be a jpg or png image.");
        }

        if (photo.Length > 6 * 1024 * 1024)
        {
            return BadRequest("The file must be less than 6 MB.");
        }

        var passenger = await _passengerRepository.GetPassengerById(id);

        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        if (passenger.FacePersonId is not null)
        {
            return Conflict("This passenger already has a registered face.");
        }

        await using var imageStream = photo.OpenReadStream();

        Guid personId;
        try
        {
            personId = await _faceRecognitionService.EnrollAsync(imageStream);
        }
        catch (AzureServiceException exception)
        {
            return StatusCode(StatusCodes.Status502BadGateway, exception.Message);
        }

        _passengerRepository.RegisterFace(id, personId);

        PassengerFaceDto mappedResult = PassengerMapper.ToPassengerFaceDto(personId.ToString());

        return Ok(mappedResult);
    }

    [HttpDelete("{id}/face")]
    public async Task<ActionResult> UnregisterFace(int id)
    {
        var passenger = await _passengerRepository.GetPassengerById(id);
        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        if (passenger.FacePersonId is null)
        {
            return BadRequest("Passenger does not have a face registered.");
        }

        try
        {
            await _faceRecognitionService.DeleteAsync(passenger.FacePersonId.Value);
        }
        catch (AzureServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }
        await _passengerRepository.UnregisterFace(id);

        return Ok();
    }
}