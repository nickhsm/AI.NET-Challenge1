using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.MappingProfiles;

public static class ConductorMapper
{
    public static ConductorProfileDto ToConductorProfileDto(this Conductor conductor)
    {
        return new ConductorProfileDto(conductor.Id, conductor.Email, conductor.FirstName, conductor.Surname);
    }
}
