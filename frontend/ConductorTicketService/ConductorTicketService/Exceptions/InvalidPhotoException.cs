namespace ConductorTicketService.Exceptions;

public class InvalidPhotoException : Exception
{
    public InvalidPhotoException(string error) : base($"Photo not valid: {error}")
    {
    }
}