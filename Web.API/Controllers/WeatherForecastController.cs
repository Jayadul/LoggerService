using Logger;
using Microsoft.AspNetCore.Mvc;

namespace Web.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly ILoggerService _loggerService;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, ILoggerService loggerService)
    {
        _logger = logger;
        _loggerService = loggerService;
    }

    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Testing Serilog: Information log");
        _logger.LogError("Testing Serilog: Error log");
        _logger.LogWarning("Testing Serilog: Warning log");
        _logger.LogDebug("Testing Serilog: Debug log");
        _logger.LogTrace("Testing Serilog: Trace log");

        try
        {
            // Simulating an exception
            throw new Exception("Simulated exception");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Testing Serilog: Exception occurred");
        }

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("notfound")]
    public IActionResult NotFoundAction()
    {
        _logger.LogInformation("Testing Serilog: NotFound action");
        return NotFound();
    }

    [HttpGet("badrequest")]
    public IActionResult BadRequestAction()
    {
        _logger.LogInformation("Testing Serilog: BadRequest action");
        return BadRequest();
    }

    [HttpGet("forbidden")]
    public IActionResult ForbiddenAction()
    {
        _logger.LogInformation("Testing Serilog: Forbidden action");
        return Forbid();
    }

    [HttpGet("unauthorized")]
    public IActionResult UnauthorizedAction()
    {
        _logger.LogInformation("Testing Serilog: Unauthorized action");
        return Unauthorized();
    }

    [HttpGet("servererror")]
    public IActionResult ServerErrorAction()
    {
        _logger.LogInformation("Testing Serilog: ServerError action");
        throw new Exception("Simulated server error");
    }

    [HttpGet("customerror/{errorCode}")]
    public IActionResult CustomErrorAction(int errorCode)
    {
        string errorMessage;
        switch (errorCode)
        {
            case 400:
                errorMessage = "Bad request";
                return BadRequest(errorMessage);
            case 401:
                errorMessage = "Unauthorized";
                return Unauthorized(errorMessage);
            case 403:
                errorMessage = "Forbidden";
                return Forbid(errorMessage);
            case 404:
                errorMessage = "Not found";
                return NotFound(errorMessage);
            default:
                errorMessage = "Internal server error";
                return StatusCode(500, errorMessage);
        }
    }
}
