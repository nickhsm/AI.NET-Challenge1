namespace TicketService.Contracts.Models;

public class TicketQrCodeDto
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string StartingStation { get; set; }
    public string DestinationStation { get; set; }
    public string FirstName { get; set; }
    public string Surname { get; set; }
    public bool Used { get; set; }
    public bool IsValidToday { get; set; }

    public TicketQrCodeDto(
        int id,
        DateOnly date,
        string startingStation,
        string destinationStation,
        string firstName,
        string surname,
        bool used
        )
    {
        Id = id;
        Date = date;
        StartingStation = startingStation;
        DestinationStation = destinationStation;
        FirstName = firstName;
        Surname = surname;
        Used = used;
        IsValidToday = date == DateOnly.FromDateTime(DateTime.Today);
    }
}