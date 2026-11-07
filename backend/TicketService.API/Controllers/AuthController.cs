using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.Services;
using TicketService.Contracts.Models;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Repositories;

namespace TicketService.API.Controllers;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private IPassengerRepository _passengerRepository;
    private IConductorRepository _conductorRepository;
    private IPasswordHasher<IUser> _hasher;
    private ITokenService _tokenService;

    public AuthController(
        IPassengerRepository passengerRepository,
        IConductorRepository conductorRepository,
        IPasswordHasher<IUser> hasher,
        ITokenService tokenService)
    {
        _passengerRepository = passengerRepository;
        _conductorRepository = conductorRepository;
        _hasher = hasher;
        _tokenService = tokenService;
    }

    private ActionResult<LoginResponseDto> CheckLogin(IUser? user, string password, string role)
    {
        if (user == null)
        {
            return Unauthorized("Email or password is incorrect.");
        }

        var result = _hasher.VerifyHashedPassword(user, user.Password, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized("Email or password is incorrect.");
        }

        return Ok(_tokenService.CreateToken(user, role));
    }

    [HttpPost("passenger/login")]
    public async Task<ActionResult<LoginResponseDto>> PassengerLogin(LoginDto loginDto)
    {
        var passenger = await _passengerRepository.GetPassengerByEmailAsync(loginDto.Email.Trim().ToLower());

        return CheckLogin(passenger, loginDto.Password, "Passenger");
    }

    [HttpPost("conductor/login")]
    public async Task<ActionResult<LoginResponseDto>> ConductorLogin(LoginDto loginDto)
    {
        var conductor = await _conductorRepository.GetConductorByEmailAsync(loginDto.Email.Trim().ToLower());

        return CheckLogin(conductor, loginDto.Password, "Conductor");
    }
}