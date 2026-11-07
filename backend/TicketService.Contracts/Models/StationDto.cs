namespace TicketService.Contracts.Models;

public class StationDto
{
    public int Id { get; set; }
    public string Name { get; set; }

    public StationDto(int id, string name)
    {
        Id = id;
        Name = name;
    }
}