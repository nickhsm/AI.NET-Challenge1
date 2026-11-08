namespace ConductorTicketService.Exceptions;

// The backend answered 409 on register or on updating the account: there is already an account with this email.
public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException() : base("An account with this email already exists")
    {
    }
}
