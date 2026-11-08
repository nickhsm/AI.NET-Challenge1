namespace TicketService.Contracts.Models;

public class TicketListDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; }
    public string DestinationStation { get; set; }

    public TicketListDto(int id, DateOnly date, string startingStation, string destinationStation)
    {
        Id = id;
        Date = date;
        StartingStation = startingStation;
        DestinationStation = destinationStation;
    }
}