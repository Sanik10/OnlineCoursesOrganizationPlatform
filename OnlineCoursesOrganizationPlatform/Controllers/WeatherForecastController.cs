using Microsoft.AspNetCore.Mvc;

namespace OnlineCoursesOrganizationPlatform.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static List<string> Summaries = new()
    {
        "Ford F-150 Trophy Truck","Ford 1979 Trophy Truck","MAZ-5309RR","KamAZ 43509K","Ford HRX","Chevrolet Pre-Runner","Acciona 100% EcoPowered","^^^^^^^^^^^^^^^^^^","все эти машины","уже в игре!","Trophy race TOP!"
    };

    //private static List<string> Summaries = new()
    //{
    //    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    //};

    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet("Get-all")]
    public List<string> Get()
    {
        return Summaries;
    }

    [HttpGet("Find-by-index")]
    public IActionResult Get(int index)
    {
        if(index < 0 || index >= Summaries.Count)
        {
            return NotFound("Элемент по этому номеру не найден");
        }
        return Ok(Summaries[index]);

    }

    [HttpGet("Find-by-name")]
    public IActionResult Search(string name)
    {
        var foundItems = Summaries.Where(item => item.Contains(name)).ToList();
        if(foundItems.Count == 0)
        {
            return NotFound("Такого имени не существует");
        }
        return Ok(foundItems);
    }

    [HttpPost("Add-element")]
    public IActionResult Add(string name)
    {
        Summaries.Add(name);
        return Ok();
    }

    [HttpPut("Change-element")]
    public IActionResult Update(int index, string name)
    {
        if(index < 0 || index >= Summaries.Count)
        {
            return BadRequest("Введен неверный индекс для изменения");
        }
        
        Summaries[index] = name;
        return Ok("Индекс удален!");
    }

    [HttpDelete("Delete-element")]
    public IActionResult Delete(int index)
    {
        if (index < 0 || index >= Summaries.Count)
        {
            return BadRequest("Введен неверный индекс для удаления");
        }
        Summaries.RemoveAt(index);
        return Ok();
    }

    [HttpGet("Sort-method")]
    public IActionResult GetAll(int? sortStrategy)
    {
        if(sortStrategy == null)
        {
            return Ok(Summaries);
        }
        else if(sortStrategy == 1)
        {
            var sortedList = Summaries.OrderBy(item => item).ToList();
            return Ok(sortedList);
        }
        else if (sortStrategy == -1)
        {
            var sortedList = Summaries.OrderByDescending(item => item).ToList();
            return Ok(sortedList);
        }
        else
        {
            return BadRequest("Некорректное значение сортировки");
        }
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