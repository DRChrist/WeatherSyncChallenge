namespace PenneoWeatherCodeChallenge.Core;

public interface IOpenWeatherClient
{
    Task<TemperatureMeasurement> GetWeather(Location? location, CancellationToken cancellationToken);
    Task<Location> GetLocation(string locationName, CancellationToken cancellationToken);
}