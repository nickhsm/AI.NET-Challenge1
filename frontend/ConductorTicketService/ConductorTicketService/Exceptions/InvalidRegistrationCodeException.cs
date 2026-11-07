namespace ConductorTicketService.Exceptions;

// The backend answered 403 on register: the registration code (normally from the invite email) is wrong.
public class InvalidRegistrationCodeException : Exception
{
    public InvalidRegistrationCodeException() : base("Registration code is incorrect")
    {
    }
}
