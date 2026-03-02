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
            _location = await openWeatherClient.GetLocation(configuration.Value.City, cancellationToken);

        var weatherResult = await GetMeasurement(cancellationToken);
        await weatherResult.Match(
            async measurement => await measurementRepository.Add(measurement, cancellationToken),
            none => { logger.LogError("Failed to fetch weather data"); return Task.CompletedTask; }
        );
    }

    private async Task<OneOf<TemperatureMeasurement, None>> GetMeasurement(CancellationToken cancellationToken)
    {
        try
        {
            return await openWeatherClient.GetWeather(_location, cancellationToken);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            // Don't rethrow the exception, keep the service running and log the error instead
            return new None();
        }
    }
    
}