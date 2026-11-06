namespace TicketService.Infrastructure.Services;

public interface IFaceRecognitionService
{
    public Task<Guid> EnrollAsync(Stream photo);
    public Task<Guid?> IdentifyAsync(Stream photo);
    public Task DeleteAsync(Guid personId);
}