using PenneoWeatherCodeChallenge.Core;
using NSubstitute;
using Microsoft.Extensions.Logging;

namespace PenneoWeatherCodeChallenge.Tests;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetWeatherData()
    {
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(new HttpClient(), new WeatherServiceConfiguration("api-key"), loggerMock);
        var result = await sut.GetWeather(new Location("TestCity", 10.0, 20.0));
        Assert.IsType<TemperatureMeasurement>(result);
    }
}
