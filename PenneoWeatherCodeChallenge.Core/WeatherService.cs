using System.Runtime.Serialization;

namespace PenneoWeatherCodeChallenge.Core;

public class WeatherService (HttpClient httpClient, WeatherServiceConfiguration config, ILogger<WeatherService> logger) : IWeatherService
{
    // public async Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken) //TODO: Implement location-based API call
    public async Task<TemperatureMeasurement> GetWeather(CancellationToken cancellationToken) //TODO: Implement location-based API call
    {
        var response = await httpClient.GetAsync($"https://api.openweathermap.com/data/2.5/weather?lat=44.34&lon=10.99&appid={config.ApiKey}");
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Successfully retrieved weather data. StatusCode: {StatusCode}", response.StatusCode);

        var openWeatherResponse = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>();
        if (openWeatherResponse == null)
        {
            throw new SerializationException("Failed to deserialize weather data from API response.");
        }
        var temperatureMeasurement = openWeatherResponse.ToMeasurement();

        return temperatureMeasurement;
    }
}