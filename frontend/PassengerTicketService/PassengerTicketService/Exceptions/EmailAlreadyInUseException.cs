namespace PassengerTicketService.Exceptions;

// The backend answered 409 on register: there is already an account with this email.
public class EmailAlreadyInUseException : Exception
{
    public EmailAlreadyInUseException() : base("Er bestaat al een account met dit e-mailadres.")
    {
    }
}
