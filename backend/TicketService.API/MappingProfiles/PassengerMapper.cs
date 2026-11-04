using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.MappingProfiles;

public static class PassengerMapper
{
    public static PassengerFaceDto ToPassengerFaceDto(this string azureFaceId)
    {
        return new PassengerFaceDto(azureFaceId);
    }

    public static CheckFaceDto ToCheckFaceDto(this Passenger passenger, bool hasTickets)
    {
        return new CheckFaceDto(passenger.FirstName, passenger.Surname, passenger.FacePersonId.ToString()!, hasTickets);
    }
}