namespace TicketService.Infrastructure.Exceptions;

public class TicketAlreadyUsedException : Exception
{
    public TicketAlreadyUsedException()
        : base("The ticket has already been used.")
    {
    }
}
