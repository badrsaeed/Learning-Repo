using ApiErrorHandlingTemplete.Errors;
using Microsoft.AspNetCore.Mvc;

namespace ApiErrorHandlingTemplete.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }


        [HttpGet("notfound")]
        public ActionResult NotFoundResponse()
        {
            return NotFound(new ApiResponse(404));
        }

        [HttpGet("badrequest")]
        public ActionResult BadRequestResponse()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("validationError/{id}")]
        public ActionResult ValidationErrorResponse(int id)
        {
            return Ok();
        }

        [HttpGet("exception")]
        public ActionResult ExcptionResponse()
        {
            throw new NullReferenceException();

            return Ok();    
        }

    }
}
