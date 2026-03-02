namespace PenneoWeatherCodeChallenge.Core;

public interface IOpenWeatherClient
{
    // Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken);
    Task<TemperatureMeasurement> GetWeather(CancellationToken cancellationToken);
}