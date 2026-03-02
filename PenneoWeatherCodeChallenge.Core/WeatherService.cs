using OneOf;
using OneOf.Types;

namespace PenneoWeatherCodeChallenge.Core;

public class WeatherService (IOpenWeatherClient openWeatherClient, MeasurementRepository measurementRepository, ILogger<WeatherService> logger)
{
    public async Task GetAndSaveWeather(CancellationToken cancellationToken)
    {
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
            return await openWeatherClient.GetWeather(cancellationToken);
        }
        catch (Exception)
        {
            // Don't rethrow the exception, keep the service running and log the error instead
            return new None();
        }
    }
    
}