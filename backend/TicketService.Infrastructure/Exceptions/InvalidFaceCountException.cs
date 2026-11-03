namespace TicketService.Infrastructure.Exceptions;

public class InvalidFaceCountException : Exception
{
    public InvalidFaceCountException(string message)
        : base(message)
    {
    }
}
