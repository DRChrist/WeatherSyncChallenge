namespace PenneoWeatherCodeChallenge.Core;

public interface IWeatherService
{
    // Task<TemperatureMeasurement> GetWeather(Location location, CancellationToken cancellationToken);
    Task<TemperatureMeasurement> GetWeather(CancellationToken cancellationToken);
}