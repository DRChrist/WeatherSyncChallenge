using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class FakeOpenWeatherClient : IOpenWeatherClient
{
    public Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken)
    {
        return Task.FromResult(new TemperatureMeasurement
        (
            20.0,
            location.Name,
            DateTime.UtcNow
        ));
    }

    public Task<Location> GetLocation(string locationName, CancellationToken cancellationToken)
    {
        return Task.FromResult(new Location(locationName, 55.6761, 12.5683));
    }
}