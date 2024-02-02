using Microsoft.AspNetCore.Mvc;

namespace OnlineCoursesOrganizationPlatform.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static List<string> Summaries = new()
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public List<string> Get()
    {
        return Summaries;
    }

    [HttpPost]
    public IActionResult Add(string name)
    {
        Summaries.Add(name);
        return Ok();
    }

    [HttpPut]
    public IActionResult Update(int index, string name)
    {
        if(index < 0 || index >= Summaries.Count)
        {
            return BadRequest("Введен неверный индекс");
        }
        
        Summaries[index] = name;
        return Ok();
    }

    [HttpDelete]
    public IActionResult Delete(int index)
    {
        Summaries.RemoveAt(index);
        return Ok();
    }

    //[HttpGet(Name = "GetWeatherForecast")]
    //public IEnumerable<WeatherForecast> Get()
    //{
    //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
    //    {
    //        Date = DateTime.Now.AddDays(index),
    //        TemperatureC = Random.Shared.Next(-20, 55),
    //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
    //    })
    //    .ToArray();
    //}
}

