using GATIntegrations.Data;
using GATIntegrations.Data.EFEntity;
using GATIntegrations.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GATIntegrations.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly GigAndTakeDbContext _dbContext;
    private readonly CommonLocalization _lc;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, CommonLocalization lc, GigAndTakeDbContext db)
    {
        _logger = logger;
        _lc = lc;
        _dbContext = db;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("GetWeatherForecast");
        Console.WriteLine(_lc.Getstr("Error.CoreWorkersRepository.CreateWorker.InvalidWorkerTypeId"));
        Console.WriteLine(_lc.Get("Error.CoreWorkersRepository.CreateWorker.InvalidWorkerTypeId"));

        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("GetWorkerType", Name = "GetWorkerType")]
    public IEnumerable<LuWorkerType> GetWorkerType()
    {
        return [.. _dbContext.LuWorkerType];
    }
}
