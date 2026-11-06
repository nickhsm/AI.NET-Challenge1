namespace ConductorTicketService.Exceptions;

public class ApiException : Exception
{
    public ApiException(string error) : base($"The backend gave an unexpected error: {error}")
    {
    }
}