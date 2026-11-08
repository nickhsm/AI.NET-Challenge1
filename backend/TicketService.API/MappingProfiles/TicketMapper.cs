using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.MappingProfiles;

public static class TicketMapper
{
    public static IEnumerable<TicketListDto> ToTicketListDto(this IEnumerable<Ticket> tickets)
    {
        List<TicketListDto> returnDto = new List<TicketListDto>();
        foreach (Ticket ticket in tickets)
        {
            returnDto.Add(new TicketListDto(
                ticket.Id, 
                ticket.ExpirationDate, 
                ticket.StartingStation!.Name,
                ticket.DestinationStation!.Name
                ));
        }

        return returnDto;
    }

    public static TicketQrCodeDto ToTicketQrCodeDto(this Ticket ticket)
    {
        return new TicketQrCodeDto(ticket.Id,
            ticket.ExpirationDate,
            ticket.StartingStation.Name,
            ticket.DestinationStation.Name,
            ticket.Passenger.FirstName,
            ticket.Passenger.Surname,
            ticket.Used
        );
    }
    
    public static IEnumerable<PassengerTicketDto> ToPassengerTicketDtos(this IEnumerable<Ticket> tickets)
    {
        var returnDto = new List<PassengerTicketDto>();
        foreach (Ticket ticket in tickets)
        {
            returnDto.Add(new PassengerTicketDto(
                ticket.Id, 
                ticket.GuidQrCode, 
                ticket.ExpirationDate, 
                ticket.StartingStation!.Name,
                ticket.DestinationStation!.Name,
                ticket.Used,    
                ticket.Price    
            ));
        }
        return returnDto;
    }
}