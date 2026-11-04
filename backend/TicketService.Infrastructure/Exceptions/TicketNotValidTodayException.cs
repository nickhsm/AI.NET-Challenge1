namespace TicketService.Infrastructure.Exceptions;

public class TicketNotValidTodayException : Exception
{
    public TicketNotValidTodayException()
        : base("The ticket is not valid today.")
    {
    }
}
