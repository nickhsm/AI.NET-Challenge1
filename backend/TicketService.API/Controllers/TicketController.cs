using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;
using TicketService.Shared;
using System.Security.Claims;  

namespace TicketService.API.Controllers;

[Route("api/ticket")]
[ApiController]
[Authorize(Roles = Roles.Conductor)]
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
    
    [Authorize(Roles = Roles.Passenger)]
    [HttpGet("passenger/me/all")]
    public async Task<ActionResult<IEnumerable<TicketListDto>>> GetMyTickets()

    {
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Haal ALLE tickets op (actief + archief)
        var tickets = await _ticketRepository.GetTicketsByPassengerIdAsync(passengerId);

        // 3. Map naar DTO
        var mapped = TicketMapper.ToPassengerTicketDtos(tickets);

        return Ok(mapped);
    }
}