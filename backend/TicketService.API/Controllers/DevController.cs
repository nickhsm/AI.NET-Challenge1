using Microsoft.AspNetCore.Mvc;
using TicketService.Infrastructure.Exceptions;
using TicketService.Infrastructure.Repositories;
using TicketService.Infrastructure.Seeders;
using TicketService.Infrastructure.Services;

namespace TicketService.API.Controllers;

// Endpoints only meant for testing. They return 404 outside Development.
[Route("api/dev")]
[ApiController]
public class DevController : ControllerBase
{
    private IDatabaseSeeder _databaseSeeder;
    private IFaceRecognitionService _faceRecognitionService;
    private IPassengerRepository _passengerRepository;
    private IWebHostEnvironment _environment;

    public DevController(IDatabaseSeeder databaseSeeder, IFaceRecognitionService faceRecognitionService,
        IPassengerRepository passengerRepository, IWebHostEnvironment environment)
    {
        _databaseSeeder = databaseSeeder;
        _faceRecognitionService = faceRecognitionService;
        _passengerRepository = passengerRepository;
        _environment = environment;
    }

    [HttpPost("seed")]
    public async Task<ActionResult> Seed()
    {
        // Act like the endpoint doesn't exist outside Development.
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        bool seeded = await _databaseSeeder.SeedAsync();

        if (!seeded)
        {
            return Conflict("The database already contains data.");
        }

        return Ok("Database seeded.");
    }

    [HttpDelete("faces")]
    public async Task<ActionResult> WipeFaces()
    {
        // Act like the endpoint doesn't exist outside Development.
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        // Azure first. If this fails, the database still matches Azure.
        // If the database update fails afterwards, calling this endpoint again is safe.
        try
        {
            await _faceRecognitionService.DeleteAllAsync();
        }
        catch (AzureServiceException ex)
        {
            return StatusCode(StatusCodes.Status502BadGateway, ex.Message);
        }

        await _passengerRepository.UnregisterAllFacesAsync();

        return NoContent();
    }
}
