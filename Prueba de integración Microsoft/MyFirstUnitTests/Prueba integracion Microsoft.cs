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

            // Cleanup
            await _client.DeleteAsync($"/weatherforecast/{newForecast.Date:yyyy-MM-dd}");
        }

        [Fact]
        public async Task Put_UpdatesWeatherForecast()
        {
            // Arrange
            var date = DateTime.Now.AddDays(2).ToString("yyyy-MM-dd");
            var initialForecast = new WeatherForecast
            {
                Date = DateTime.Now.AddDays(2),
                TemperatureC = 20,
                Summary = "Warm"
            };
            var updatedForecast = new WeatherForecast
            {
                Date = initialForecast.Date,
                TemperatureC = 30,
                Summary = "Sweltering"
            };
            var initialContent = new StringContent(JsonSerializer.Serialize(initialForecast), Encoding.UTF8, "application/json");
            var updatedContent = new StringContent(JsonSerializer.Serialize(updatedForecast), Encoding.UTF8, "application/json");

            // Act - Add initial forecast
            var postResponse = await _client.PostAsync("/weatherforecast", initialContent);
            postResponse.EnsureSuccessStatusCode();

            // Act - Update forecast
            var putResponse = await _client.PutAsync($"/weatherforecast/{date}", updatedContent);
            putResponse.EnsureSuccessStatusCode();

            // Verify update
            var getResponse = await _client.GetAsync("/weatherforecast");
            var responseString = await getResponse.Content.ReadAsStringAsync();

            // Deserializar la respuesta para verificar la actualización
            var forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(responseString);

            // Debugging output
            Console.WriteLine($"Response String: {responseString}");

            // Verificar que el pronóstico actualizado está presente
            var updatedForecastFromResponse = forecasts.FirstOrDefault(f => f.Date.ToString("yyyy-MM-dd") == date);
            Assert.NotNull(updatedForecastFromResponse);
            Assert.Equal("Sweltering", updatedForecastFromResponse.Summary);
            Assert.Equal(30, updatedForecastFromResponse.TemperatureC);

            // Cleanup
            await _client.DeleteAsync($"/weatherforecast/{date}");
        }


        [Fact]
        public async Task Delete_RemovesWeatherForecast()
        {
            // Arrange
            var dateToDelete = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");

            // Act
            var response = await _client.DeleteAsync($"/weatherforecast/{dateToDelete}");

            // Assert
            response.EnsureSuccessStatusCode();

            // Verify deletion
            var getResponse = await _client.GetAsync("/weatherforecast");
            var responseString = await getResponse.Content.ReadAsStringAsync();

            // Deserializar la respuesta para verificar que la fecha ha sido eliminada
            var forecasts = JsonSerializer.Deserialize<List<WeatherForecast>>(responseString);

            // Verificar que ningún pronóstico contiene la fecha eliminada
            Assert.DoesNotContain(forecasts, f => f.Date.ToString("yyyy-MM-dd") == dateToDelete);
        }


    }
}
