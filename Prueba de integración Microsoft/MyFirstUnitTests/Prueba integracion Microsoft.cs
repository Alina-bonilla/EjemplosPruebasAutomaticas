using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ApiEjemplo;


namespace MyFirstUnitTests
{
    public class WeatherForecastIntegrationTests : IClassFixture<WebApplicationFactory<ApiEjemplo.TestStartup>>
    {
        private readonly HttpClient _client;

        public WeatherForecastIntegrationTests(WebApplicationFactory<ApiEjemplo.TestStartup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Get_ReturnsWeatherForecast()
        {
            // Act
            var response = await _client.GetAsync("/weatherforecast");

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            var responseString = await response.Content.ReadAsStringAsync();

            // Lista de cadenas para verificar en la respuesta
            var expectedStrings = new[] { "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching" };

            // Verificar que al menos una cadena de la lista está presente en la respuesta
            Assert.True(expectedStrings.Any(s => responseString.Contains(s)), "The response does not contain any of the expected strings.");
        }

        [Fact]
        public async Task Post_AddsWeatherForecast()
        {
            // Arrange
            var newForecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = 25,
                Summary = "Hot"
            };
            var content = new StringContent(JsonSerializer.Serialize(newForecast), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/weatherforecast", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Hot", responseString);
        }

        [Fact]
        public async Task Put_UpdatesWeatherForecast()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            var updatedForecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(1),
                TemperatureC = 30,
                Summary = "Sweltering"
            };
            var content = new StringContent(JsonSerializer.Serialize(updatedForecast), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PutAsync($"/weatherforecast/{date}", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task Delete_RemovesWeatherForecast()
        {
            // Arrange
            var date = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            // Act
            var response = await _client.DeleteAsync($"/weatherforecast/{date}");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}

