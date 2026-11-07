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
    private IConfiguration _configuration;

    public AuthController(
        IPassengerRepository passengerRepository,
        IConductorRepository conductorRepository,
        IPasswordHasher<IUser> hasher,
        ITokenService tokenService,
        IConfiguration configuration)
    {
        _passengerRepository = passengerRepository;
        _conductorRepository = conductorRepository;
        _hasher = hasher;
        _tokenService = tokenService;
        _configuration = configuration;
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

    [HttpPost("passenger/register")]
    public async Task<ActionResult<LoginResponseDto>> PassengerRegister(RegisterDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var email = registerDto.Email.Trim().ToLower();

        if (await _passengerRepository.GetPassengerByEmailAsync(email) != null)
        {
            return Conflict("An account with this email already exists.");
        }

        var passenger = new Passenger(email, registerDto.Password)
        {
            FirstName = registerDto.FirstName,
            Surname = registerDto.Surname,
            DateOfBirth = registerDto.DateOfBirth
        };
        passenger.Password = _hasher.HashPassword(passenger, registerDto.Password);

        await _passengerRepository.AddPassengerAsync(passenger);

        return Ok(_tokenService.CreateToken(passenger, "Passenger"));
    }

    [HttpPost("conductor/register")]
    public async Task<ActionResult<LoginResponseDto>> ConductorRegister(ConductorRegisterDto registerDto)
    {
        // Check the code first, so someone without a code can't find out which emails already have an account.
        var expectedCode = _configuration["Registration:ConductorCode"];

        if (string.IsNullOrEmpty(expectedCode) || registerDto.RegistrationCode?.Trim() != expectedCode)
        {
            return StatusCode(StatusCodes.Status403Forbidden, "Registration code is incorrect.");
        }

        if (string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var email = registerDto.Email.Trim().ToLower();

        if (await _conductorRepository.GetConductorByEmailAsync(email) != null)
        {
            return Conflict("An account with this email already exists.");
        }

        var conductor = new Conductor(email, registerDto.Password)
        {
            FirstName = registerDto.FirstName,
            Surname = registerDto.Surname,
            DateOfBirth = registerDto.DateOfBirth
        };
        conductor.Password = _hasher.HashPassword(conductor, registerDto.Password);

        await _conductorRepository.AddConductorAsync(conductor);

        return Ok(_tokenService.CreateToken(conductor, "Conductor"));
    }
}