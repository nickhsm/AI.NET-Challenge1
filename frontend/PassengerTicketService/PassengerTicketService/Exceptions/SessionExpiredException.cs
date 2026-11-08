namespace PassengerTicketService.Exceptions;

// The backend answered 401: AuthHeaderHandler already logged the user out and went to the start page.
public class SessionExpiredException : Exception
{
    public SessionExpiredException() : base("Uw sessie is verlopen. Log opnieuw in.")
    {
    }
}
