namespace TicketService.Infrastructure.Exceptions;

public class AzureServiceException : Exception
{
    public AzureServiceException(string error)
        : base($"Azure request failed: {error}")
    {
    }
}
