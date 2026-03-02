using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class OpenWeatherClientTests
{
    [Fact(Skip = "Only for manually testing OpenWeatherClient.")]
    public async Task SimpleClientRequest()
    {
        var httpClient = new HttpClient();
        var configuration = Options.Create(new OpenWeatherClientConfiguration { ApiKey = "<YOUR_API_KEY>" });
        var sut = new OpenWeatherClient(httpClient, configuration);
        var result = await sut.GetWeather(new Location("Copenhagen", 44.34, 10.99), CancellationToken.None);

        Console.WriteLine(result.Temperature);
    }

    [Fact]
    public async Task GetLocation_LatitudeAndLongitude_AreDeserializedCorrectly()
    {
        // Arrange
        var json = """[{"name":"Copenhagen","lat":55.6761,"lon":12.5683}]""";
        var httpClient = new HttpClient(new FakeHttpMessageHandler(json))
        {
            BaseAddress = new Uri("https://api.openweathermap.org/")
        };
        var configuration = Options.Create(new OpenWeatherClientConfiguration { ApiKey = "test" });
        var sut = new OpenWeatherClient(httpClient, configuration);

        // Act
        var location = await sut.GetLocation("Copenhagen", CancellationToken.None);

        // Assert
        Assert.Equal("Copenhagen", location.Name);
        Assert.Equal(55.6761, location.Latitude);
        Assert.Equal(12.5683, location.Longitude);
    }

    private sealed class FakeHttpMessageHandler(string responseJson) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            });
    }
}
