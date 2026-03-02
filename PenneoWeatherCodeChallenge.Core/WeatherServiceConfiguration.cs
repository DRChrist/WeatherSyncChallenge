namespace PenneoWeatherCodeChallenge.Core;

public record WeatherServiceConfiguration
{
    public string City { get; init; } = string.Empty;
}