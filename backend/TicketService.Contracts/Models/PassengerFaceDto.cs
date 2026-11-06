namespace TicketService.Contracts.Models;

public class PassengerFaceDto
{
    public string AzureFaceId { get; set; }

    public PassengerFaceDto(string azureFaceId)
    {
        AzureFaceId = azureFaceId;
    }
}