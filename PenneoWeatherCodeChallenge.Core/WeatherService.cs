using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;

namespace PenneoWeatherCodeChallenge.Core;

public class WeatherService (IOpenWeatherClient openWeatherClient,
                             MeasurementRepository measurementRepository,
                             IOptions<WeatherServiceConfiguration> configuration,
                             ILogger<WeatherService> logger)
{
    private Location _location = new(string.Empty, 0, 0); // Default value, will be updated on first fetch

    public async Task GetAndSaveWeather(CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_location.Name))
        {
            var city = configuration.Value.City ?? "Copenhagen";
            _location = await openWeatherClient.GetLocation(city, cancellationToken);
        }

        var weatherResult = await GetMeasurement(cancellationToken);
        weatherResult.Switch(async
            measurement => await measurementRepository.SaveMeasurement(measurement, cancellationToken),
            none => logger.LogError("Failed to fetch weather data")
        );
    }

    private async Task<OneOf<TemperatureMeasurement, None>> GetMeasurement(CancellationToken cancellationToken)
    {
        try
        {
            return await openWeatherClient.GetWeather(_location, cancellationToken);
        }
        catch (Exception)
        {
            // Don't rethrow the exception, keep the service running and log the error instead
            return new None();
        }
    }
    
}