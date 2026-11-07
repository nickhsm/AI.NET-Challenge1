namespace ConductorTicketService.Exceptions;

public class TicketNotFoundException : Exception
{
    public TicketNotFoundException(string error) : base($"Ticket not found: {error}")
    {
    }
}
