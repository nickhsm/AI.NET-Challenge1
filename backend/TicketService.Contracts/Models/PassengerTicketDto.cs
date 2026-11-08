namespace TicketService.Contracts.Models;

public class PassengerTicketDto
{
    public int Id { get; set; }
    public Guid GuidQrCode { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; }
    public string DestinationStation { get; set; }
    public bool Used { get; set; }
    public decimal Price { get; set; }

    public PassengerTicketDto(int id, Guid guidQrCode, DateOnly date, string startingStation, string destinationStation, bool used, decimal price)
    {
        Id = id;
        GuidQrCode = guidQrCode;
        Date = date;
        StartingStation = startingStation;
        DestinationStation = destinationStation;
        Used = used;
        Price = price;
    }
}