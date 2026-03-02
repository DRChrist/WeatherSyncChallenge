using System.Runtime.Serialization;
using Microsoft.Extensions.Options;

namespace PenneoWeatherCodeChallenge.Core;

public class OpenWeatherClient(HttpClient httpClient, IOptions<OpenWeatherClientConfiguration> configuration) : IOpenWeatherClient
{
    public async Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"/data/2.5/weather?lat={location.Latitude}&lon={location.Longitude}&appid={configuration.Value.ApiKey}&units=metric", cancellationToken);
        response.EnsureSuccessStatusCode();

        var openWeatherResponse = await response.Content.ReadFromJsonAsync<OpenWeatherResponse>(cancellationToken: cancellationToken) 
            ?? throw new SerializationException("Failed to deserialize weather data from API response.");
        var temperatureMeasurement = openWeatherResponse.ToMeasurement();

        return temperatureMeasurement;
    }

    public async Task<Location> GetLocation(string locationName, CancellationToken cancellationToken)
    {
        var response = await httpClient.GetAsync($"/geo/1.0/direct?q={Uri.EscapeDataString(locationName)}&limit=1&appid={configuration.Value.ApiKey}", cancellationToken);
        response.EnsureSuccessStatusCode();

        var locations = await response.Content.ReadFromJsonAsync<List<Location>>(cancellationToken: cancellationToken)
            ?? throw new SerializationException("Geocoding API returned null.");

        if (locations.Count == 0) throw new InvalidOperationException($"No location found for name: {locationName}");

        return locations[0];
    }
}