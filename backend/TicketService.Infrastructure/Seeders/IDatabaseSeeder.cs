namespace TicketService.Infrastructure.Seeders;

public interface IDatabaseSeeder
{
    // Returns false when the database already contains data, so nothing was seeded.
    public Task<bool> SeedAsync();
}
