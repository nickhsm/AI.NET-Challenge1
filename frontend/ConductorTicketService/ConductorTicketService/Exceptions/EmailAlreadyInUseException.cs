namespace ConductorTicketService.Exceptions;

// The backend answered 409 on register: there is already an account with this email.
public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException() : base("An account with this email already exists")
    {
    }
}
