using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class FakeOpenWeatherClient : IOpenWeatherClient
{
    public Task<TemperatureMeasurement> GetWeather(CancellationToken cancellationToken)
    {
        return Task.FromResult(new TemperatureMeasurement
        (
            20.0,
            new Location("TestCity", 10.0, 20.0),
            DateTime.UtcNow
        ));
    }
}