using OneOf;
using OneOf.Types;

namespace PenneoWeatherCodeChallenge.Core;

public class WeatherPollingService(
    IWeatherService weatherService,
    MeasurementRepository measurementRepository,
    ILogger<WeatherPollingService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        do
        {
            var temperatureMeasurement = await GetMeasurement(stoppingToken);
            temperatureMeasurement.Switch(async
                measurement => await measurementRepository.SaveMeasurement(temperatureMeasurement.AsT0, stoppingToken),
                none => logger.LogError("Failed to fetch weather data")
            );
        } 
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task<OneOf<TemperatureMeasurement, None>> GetMeasurement(CancellationToken stoppingToken)
    {
        try
        {
            return await weatherService.GetWeather(stoppingToken);
        }
        catch (Exception)
        {
            // Don't rethrow — we want the loop to continue
            return new None();
        }
    }
}