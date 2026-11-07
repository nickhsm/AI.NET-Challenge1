namespace ConductorTicketService.Services;

public interface INavigationService
{
    Task NavigateToScanQrCodePageAsync();
    Task NavigateToScanFacePageAsync();
    Task NavigateBackAsync();
    Task NavigateToMainPageAsync();
    Task NavigateToLoginPageAsync();
}