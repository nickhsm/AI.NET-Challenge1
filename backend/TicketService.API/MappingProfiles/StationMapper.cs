using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.MappingProfiles;

public static class StationMapper
{
    public static StationDto ToStationDto(this Station station)
    {
        return new StationDto(station.Id, station.Name);
    }
}