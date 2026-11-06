namespace TicketService.Infrastructure.Exceptions;

public class TicketNotFoundException : Exception
{
    public TicketNotFoundException()
        : base("The ticket was not found.")
    {
    }
}
