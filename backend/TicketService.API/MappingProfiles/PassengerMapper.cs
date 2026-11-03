using TicketService.Contracts.Models;

namespace TicketService.API.MappingProfiles;

public static class PassengerMapper
{
    public static PassengerFaceDto ToPassengerFaceDto(this string azureFaceId)
    {
        return new PassengerFaceDto(azureFaceId);
    }
}