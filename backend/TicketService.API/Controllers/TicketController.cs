using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;

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
    
    // Voeg deze toe aan je bestaande TicketController
    [HttpGet("passenger/{passengerId}/all")]
    public async Task<ActionResult<IEnumerable<TicketListDto>>> GetTicketsByPassengerId(int passengerId)
    {
        // 1. Check of de passagier bestaat
        var passengerExists = await _passengerRepository.PassengerExistsAsync(passengerId);
        if (!passengerExists)
        {
            return NotFound("Passenger does not exist.");
        }

        // 2. Haal ALLE tickets op (actief + archief)
        var tickets = await _ticketRepository.GetTicketsByPassengerIdAsync(passengerId);

        // 3. Map naar DTO
        var mapped = TicketMapper.ToTicketListDto(tickets);

        return Ok(mapped);
    }
}