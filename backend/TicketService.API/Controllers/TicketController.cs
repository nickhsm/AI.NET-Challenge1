using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;
using TicketService.Shared;

namespace TicketService.API.Controllers;

[Route("api/ticket")]
[ApiController]
public class TicketController : ControllerBase
{
    private ITicketRepository  _ticketRepository;
    private IPassengerRepository _passengerRepository;

    public TicketController(ITicketRepository ticketRepository, IPassengerRepository passengerRepository)
    {
        _ticketRepository = ticketRepository;
        _passengerRepository = passengerRepository;
    }

    private int GetPassengerId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [Authorize(Roles = Roles.Conductor)]
    [HttpGet("passenger/{FacePersonId:guid}")]
    public async Task<ActionResult<IEnumerable<TicketListDto>>> GetPassengerTickets(Guid facePersonId)
    {
        var passenger = await _passengerRepository.GetPassengerByFacePersonIdAsync(facePersonId);

        if (passenger is null)
        {
            return NotFound("Passenger does not exist.");
        }

        var tickets = await _ticketRepository.GetValidTicketsAsync(facePersonId);

        var mapped = TicketMapper.ToTicketListDto(tickets);

        return Ok(mapped);
    }

    [Authorize(Roles = Roles.Conductor)]
    [HttpPatch("consume/{id}")]
    public async Task<ActionResult> ConsumeTicket(int id)
    {
        try
        {
            await _ticketRepository.ConsumeTicketAsync(id);
        }
        catch (TicketNotFoundException)
        {
            return NotFound("Ticket does not exist.");
        }
        catch (TicketAlreadyUsedException)
        {
            return Conflict("Ticket is already used.");
        }
        catch (TicketNotValidTodayException)
        {
            return Conflict("Ticket is not valid for today.");
        }

        return NoContent();
    }

    [Authorize(Roles = Roles.Conductor)]
    [HttpGet("qr/{qrCode:guid}")]
    public async Task<ActionResult<TicketQrCodeDto>> GetTicketByQrCode(Guid qrCode)
    {
        var result = await _ticketRepository.GetTicketByQrCodeAsync(qrCode);

        if (result is null)
        {
            return NotFound("Ticket does not exist.");
        }

        return Ok(TicketMapper.ToTicketQrCodeDto(result));
    }

    [Authorize(Roles = Roles.Passenger)]
    [HttpGet("passenger/me/all")]
    public async Task<ActionResult<IEnumerable<PassengerTicketDto>>> GetMyTickets()
    {
        int passengerId = GetPassengerId();

        var tickets = await _ticketRepository.GetTicketsByPassengerIdAsync(passengerId);

        var mapped = TicketMapper.ToPassengerTicketDtos(tickets);

        return Ok(mapped);
    }

    [HttpPost("calculate-price")]
    public ActionResult<PriceCalculationResponseDto> CalculatePrice([FromBody] PriceCalculationRequestDto request)
    {
        // Validatie: je kunt geen ticket voor 0 of -1 personen kopen
        if (request.PassengerCount <= 0)
        {
            return BadRequest("Het aantal passagiers moet minimaal 1 zijn.");
        }

        // BUSINESS LOGIC
        // Basisprijs voor een enkele reis per persoon
        decimal basePrice = 2.85m; 
        
        decimal totalPrice = basePrice * request.PassengerCount;

        // Als het een retour is, verdubbelt de prijs
        if (request.IsRoundTrip)
        {
            totalPrice *= 2;
        }

        var response = new PriceCalculationResponseDto(totalPrice);
        
        return Ok(response);
    }
}
