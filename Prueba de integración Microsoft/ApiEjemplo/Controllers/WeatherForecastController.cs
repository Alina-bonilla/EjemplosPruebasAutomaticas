using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace ApiEjemplo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private static readonly List<WeatherForecast> Forecasts = GenerateForecasts();

        private static List<WeatherForecast> GenerateForecasts()
        {
            var random = new Random();
            var forecasts = new List<WeatherForecast>();
            var index = 0;

            foreach (var summary in Summaries)
            {
                var randomDate = DateTime.Now.Date.AddDays(index);
                var formattedDate = randomDate.ToString("yyyy-MM-dd");
                index++;

                if (DateTime.TryParseExact(formattedDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
                {
                    forecasts.Add(new WeatherForecast
                    {
                        Date = parsedDate,
                        TemperatureC = random.Next(-20, 55),
                        Summary = summary
                    });
                }
            }

            return forecasts;
        }

        public WeatherForecastController(){  }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            return Forecasts;
        }

        [HttpPost]
        public IActionResult Post([FromBody] WeatherForecast forecast)
        {
            Forecasts.Add(forecast);
            return CreatedAtAction(nameof(Get), new { date = forecast.Date }, forecast);
        }

        [HttpPut("{date}")]
        public IActionResult Put(DateTime date, [FromBody] WeatherForecast updatedForecast)
        {
            var forecast = Forecasts.Find(f => f.Date == date);
            if (forecast == null)
            {
                return NotFound();
            }

            forecast.TemperatureC = updatedForecast.TemperatureC;
            forecast.Summary = updatedForecast.Summary;

            return NoContent();
        }

        [HttpDelete("{date}")]
        public IActionResult Delete(DateTime date)
        {
            var forecast = Forecasts.Find(f => f.Date == date);
            if (forecast == null)
            {
                return NotFound();
            }

            Forecasts.Remove(forecast);
            return NoContent();
        }
    }
}
