namespace ConductorTicketService.Exceptions;

// The backend answered 403 on change password: the current password is not correct.
public class WrongPasswordException : Exception
{
    public WrongPasswordException() : base("Current password is incorrect")
    {
    }
}
