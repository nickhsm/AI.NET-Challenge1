namespace ConductorTicketService.Exceptions;

public class PassengerNotFoundException : Exception
{
    public PassengerNotFoundException(string error) : base($"Passenger not found: {error}")
    {
    }
}
