namespace TicketService.Infrastructure.Services;

public interface IFaceRecognitionService
{
    public Task<Guid> EnrollAsync(Stream photo);
    public Task<Guid?> IdentifyAsync(Stream photo);
    public Task DeleteAsync(Guid personId);
    // Only for development. Deletes every face by recreating the whole group.
    public Task DeleteAllAsync();
}