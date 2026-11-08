using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Domain;
using TicketService.Domain.Entities;
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
    private IStationRepository _stationRepository;

    public TicketController(ITicketRepository ticketRepository, IPassengerRepository passengerRepository, IStationRepository stationRepository)
    {
        _ticketRepository = ticketRepository;
        _passengerRepository = passengerRepository;
        _stationRepository = stationRepository;
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
        // 1. Haal het ID veilig uit het token
        int passengerId = GetPassengerId();

        // 2. Haal ALLE tickets op (actief + archief)
        var tickets = await _ticketRepository.GetTicketsByPassengerIdAsync(passengerId);

        // 3. Map naar DTO
        var mapped = TicketMapper.ToPassengerTicketDtos(tickets);

        return Ok(mapped);
    }

    [Authorize(Roles = Roles.Passenger)]
    [HttpPost("calculate-price")]
    public ActionResult<PriceCalculationResponseDto> CalculatePrice([FromBody] PriceCalculationRequestDto request)
    {
        decimal outboundPrice = TicketPricing.CalculateOutboundPrice(request.PassengerCount);
        decimal totalPrice = TicketPricing.CalculateTotalPrice(request.PassengerCount, request.IsRoundTrip);

        var response = new PriceCalculationResponseDto(outboundPrice, totalPrice);

        return Ok(response);
    }
    
    [Authorize(Roles = Roles.Passenger)]
    [HttpPost("purchase")]
    public async Task<ActionResult<IEnumerable<TicketListDto>>> PurchaseTickets([FromBody] PurchaseTicketRequestDto request)
    {
        int passengerId = GetPassengerId();

        // 1. Controleer of de passagier bestaat
        var passengerExists = await _passengerRepository.PassengerExistsAsync(passengerId);
        if (!passengerExists)
            return NotFound("Passenger does not exist.");

        // 2. Controleer of de stations bestaan (met een simpele check)
        var startStation = await _stationRepository.GetStationByIdAsync(request.StartStationId);
        var endStation = await _stationRepository.GetStationByIdAsync(request.EndStationId);

        if (startStation == null || endStation == null)
            return BadRequest("Ongeldige stations opgegeven.");

        // Een ontbrekende datum wordt 0001-01-01 en valt dus ook onder deze check.
        if (request.ExpirationDate < DateOnly.FromDateTime(DateTime.Now))
            return BadRequest("De reisdatum mag niet in het verleden liggen.");

        // Een ontbrekende retourdatum wordt 0001-01-01 en valt dus ook onder deze check.
        if (request.IsRoundTrip && request.ReturnExpirationDate < request.ExpirationDate)
            return BadRequest("De retourdatum mag niet voor de heenreisdatum liggen.");

        // 3. Prijsberekening
        decimal basePrice = 2.85m;

        var newTickets = new List<Ticket>();

        // 4. Maak de tickets aan
        for (int i = 0; i < request.PassengerCount; i++)
        {
            // Heenreis Ticket (Altijd de basisprijs)
            var outboundTicket = new Ticket(
                request.ExpirationDate,
                passengerId,
                request.StartStationId,
                request.EndStationId,
                basePrice // <-- AANGEPAST naar basePrice
            );
            newTickets.Add(outboundTicket);

            // Terugreis Ticket (Alleen als het een retour is)
            if (request.IsRoundTrip)
            {
                var returnTicket = new Ticket(
                    request.ReturnExpirationDate,
                    passengerId,
                    request.EndStationId,
                    request.StartStationId,
                    basePrice // <-- Dit is al goed
                );
                newTickets.Add(returnTicket);
            }
        }

        await _ticketRepository.AddTicketsAsync(newTickets);
        var mappedTickets = TicketMapper.ToTicketListDto(newTickets);

        return Ok(mappedTickets);
    }
}