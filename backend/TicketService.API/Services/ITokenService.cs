using TicketService.Contracts.Models;
using TicketService.Domain.Entities;

namespace TicketService.API.Services;

public interface ITokenService
{
    LoginResponseDto CreateToken(IUser user, string role);
}