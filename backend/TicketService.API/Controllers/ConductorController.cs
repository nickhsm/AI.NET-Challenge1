using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TicketService.API.MappingProfiles;
using TicketService.Contracts.Models;
using TicketService.Domain.Entities;
using TicketService.Infrastructure.Repositories;
using TicketService.Shared;

namespace TicketService.API.Controllers;

[Route("api/conductor")]
[ApiController]
[Authorize(Roles = Roles.Conductor)]
public class ConductorController : ControllerBase
{
    private IConductorRepository _conductorRepository;
    private IPasswordHasher<IUser> _hasher;

    public ConductorController(IConductorRepository conductorRepository, IPasswordHasher<IUser> hasher)
    {
        _conductorRepository = conductorRepository;
        _hasher = hasher;
    }

    private int GetConductorId()
    {
        return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }

    [HttpGet("me")]
    public async Task<ActionResult<ConductorProfileDto>> GetMyProfile()
    {
        var conductor = await _conductorRepository.GetConductorByIdAsync(GetConductorId());

        if (conductor == null)
        {
            return NotFound("Conductor does not exist.");
        }

        return Ok(conductor.ToConductorProfileDto());
    }

    [HttpPut("me")]
    public async Task<ActionResult> UpdateMyProfile(UpdateConductorProfileDto updateDto)
    {
        if (string.IsNullOrWhiteSpace(updateDto.Email))
        {
            return BadRequest("Email is required.");
        }

        int id = GetConductorId();
        // Same normalization as login and register, otherwise the new email could never be used to log in.
        var email = updateDto.Email.Trim().ToLower();

        // Only a problem when the email belongs to someone else. Keeping your own email is fine.
        var emailOwner = await _conductorRepository.GetConductorByEmailAsync(email);
        if (emailOwner != null && emailOwner.Id != id)
        {
            return Conflict("An account with this email already exists.");
        }

        bool updated = await _conductorRepository.UpdateConductorProfileAsync(
            id, email, updateDto.FirstName?.Trim(), updateDto.Surname?.Trim());

        if (!updated)
        {
            return NotFound("Conductor does not exist.");
        }

        return NoContent();
    }

    [HttpPatch("me/password")]
    public async Task<ActionResult> ChangeMyPassword(ConductorChangePasswordDto passwordDto)
    {
        if (string.IsNullOrWhiteSpace(passwordDto.NewPassword))
        {
            return BadRequest("New password is required.");
        }

        var conductor = await _conductorRepository.GetConductorByIdAsync(GetConductorId());

        if (conductor == null)
        {
            return NotFound("Conductor does not exist.");
        }

        var result = _hasher.VerifyHashedPassword(conductor, conductor.Password, passwordDto.CurrentPassword);

        if (result == PasswordVerificationResult.Failed)
        {
            // 403 and not 401: the token is valid, only the password check failed.
            // A 401 would make the app think the conductor is logged out.
            return StatusCode(StatusCodes.Status403Forbidden, "Current password is incorrect.");
        }

        string hashedPassword = _hasher.HashPassword(conductor, passwordDto.NewPassword);
        await _conductorRepository.UpdateConductorPasswordAsync(conductor.Id, hashedPassword);

        return NoContent();
    }
}
