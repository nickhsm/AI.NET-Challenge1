namespace ConductorTicketService.Exceptions;

public class TicketNotConsumableException : Exception
{
    public TicketNotConsumableException(string error) : base($"Ticket cannot be consumed: {error}")
    {
    }
}
