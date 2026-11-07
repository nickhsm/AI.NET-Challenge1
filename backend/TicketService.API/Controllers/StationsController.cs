using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles; // <-- Vergeet deze using niet!
using TicketService.Contracts.Models;
using TicketService.Infrastructure.Repositories;

namespace TicketService.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StationsController : ControllerBase
{
    private readonly IStationRepository _stationRepository;

    public StationsController(IStationRepository stationRepository)
    {
        _stationRepository = stationRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<StationDto>>> GetStations([FromQuery] string? searchQuery)
    {
        var stations = await _stationRepository.GetStationsAsync(searchQuery);

        // Gebruik de extension method voor de mapping
        var stationDtos = stations.Select(s => s.ToStationDto()).ToList();

        return Ok(stationDtos);
    }
}