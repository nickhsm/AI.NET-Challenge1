using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.MappingProfiles;

public static class StationMapper
{
    public static IEnumerable<StationDto> ToStationDtos(this IEnumerable<Station> stations)
    {
        var result = new List<StationDto>();
        foreach (var station in stations)
        {
            result.Add(new StationDto(station.Id, station.Name));
        }
        return result;
    }
}