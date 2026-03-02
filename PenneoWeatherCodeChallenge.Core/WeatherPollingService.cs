

namespace PenneoWeatherCodeChallenge.Core;

public class WeatherPollingService(WeatherService weatherService) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        do
        {
            await weatherService.GetAndSaveWeather(stoppingToken);
        } 
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}