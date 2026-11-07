namespace ConductorTicketService.Exceptions;

public class InvalidLoginException : Exception
{
    public InvalidLoginException() : base("Email or password is incorrect")
    {
    }
}