using System.Runtime.Serialization;
using Microsoft.Extensions.Options;

namespace PenneoWeatherCodeChallenge.Core;

public class OpenWeatherClient(HttpClient httpClient, IOptions<OpenWeatherClientConfiguration> configuration, ILogger<OpenWeatherClient> logger) : IOpenWeatherClient
{
    // TODO: double-check error handling and logging.
    public async Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"https://api.openweathermap.com/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={configuration.Value.ApiKey}&units=metric", cancellationToken);
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Successfully retrieved weather data. StatusCode: {StatusCode}", response.StatusCode);

        var openWeatherResponse = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(cancellationToken: cancellationToken) 
            ?? throw new SerializationException("Failed to deserialize weather data from API response.");
        var temperatureMeasurement = openWeatherResponse.ToMeasurement();

        return temperatureMeasurement;
    }

    public async Task<Location> GetLocation(string locationName, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"https://api.openweathermap.org/geo/1.0/direct?q={locationName}&limit=1&appid={configuration.Value.ApiKey}", cancellationToken);
        response.EnsureSuccessStatusCode();

        logger.LogInformation("Successfully retrieved location data. StatusCode: {StatusCode}", response.StatusCode);

        var locations = await response.Content.ReadFromJsonAsync<List<Location>>(cancellationToken: cancellationToken) 
            ?? throw new SerializationException("Failed to deserialize location data from API response.");

        if (locations.Count == 0)
        {
            throw new Exception($"No location found for name: {locationName}");
        }

        return locations[0];
    }
}