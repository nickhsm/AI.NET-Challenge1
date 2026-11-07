namespace PassengerTicketService.Exceptions;

// The backend answered with an error we don't handle separately.
public class ApiException : Exception
{
    public ApiException(string error) : base($"De server gaf een onverwachte fout: {error}")
    {
    }
}
