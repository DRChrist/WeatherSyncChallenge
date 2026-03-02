using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class OpenWeatherClientTests
{
    [Fact (Skip = "Only for manually testing OpenWeatherClient.")]
    public async Task SimpleClientRequest()
    {
        var httpClient = new HttpClient();
        var configuration = Options.Create(new OpenWeatherClientConfiguration("<YOUR_API_KEY>"));
        var logger = Substitute.For<ILogger<OpenWeatherClient>>();
        var sut = new OpenWeatherClient(httpClient, configuration, logger);
        var result = await sut.GetWeather(new Location("Copenhagen", 44.34, 10.99), CancellationToken.None);

        Console.WriteLine(result.Temperature);
    }
}