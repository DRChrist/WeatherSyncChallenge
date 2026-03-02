using PenneoWeatherCodeChallenge.Core;
using NSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;
using Microsoft.Extensions.Options;
using Microsoft.Data.Sqlite;

namespace PenneoWeatherCodeChallenge.Tests;

public class WeatherServiceTests
{
    private static (MeasurementRepository repo, SqliteConnection anchor) CreateRepository()
    {
        var connectionString = $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared";
        var anchor = new SqliteConnection(connectionString);
        anchor.Open();
        var config = Options.Create(new MeasurementRepositoryConfiguration { ConnectionString = connectionString });
        var repo = new MeasurementRepository(config);
        repo.InitializeDatabase();
        return (repo, anchor);
    }

    [Fact]
    public async Task GetMeasurement_HappyPath()
    {
        // Arrange
        var (repo, anchor) = CreateRepository();
        using var _ = anchor;
        var fakeOpenWeatherClient = new FakeOpenWeatherClient();
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var weatherServiceConfig = Options.Create(new WeatherServiceConfiguration { City = "Copenhagen" });
        var sut = new WeatherService(fakeOpenWeatherClient, repo, weatherServiceConfig, loggerMock);

        // Act
        await sut.GetAndSaveWeather(CancellationToken.None);

        // Assert
        var measurements = await repo.GetAll(CancellationToken.None);
        Assert.NotEmpty(measurements);
    }

    [Fact]
    public async Task GetMeasurement_WeatherServiceThrows_ShouldLogError()
    {
        // Arrange
        var (repo, anchor) = CreateRepository();
        using var _ = anchor;
        var openWeatherClientMock = Substitute.For<IOpenWeatherClient>();
        openWeatherClientMock.GetWeather(Arg.Any<Location>(), Arg.Any<CancellationToken>()).Throws(new Exception("API error"));
        var weatherServiceConfig = Options.Create(new WeatherServiceConfiguration { City = "Copenhagen" });
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(openWeatherClientMock, repo, weatherServiceConfig, loggerMock);

        // Act
        await sut.GetAndSaveWeather(CancellationToken.None);

        // Assert
        loggerMock.Received().LogError("Failed to fetch weather data");
        var measurements = await repo.GetAll(CancellationToken.None);
        Assert.Empty(measurements);
    }

    [Fact]
    public async Task GetMeasurement_CancellationRequested_ShouldNotSaveMeasurement()
    {
        // Arrange
        var (repo, anchor) = CreateRepository();
        using var _ = anchor;
        var fakeOpenWeatherClient = new FakeOpenWeatherClient();
        var weatherServiceConfig = Options.Create(new WeatherServiceConfiguration { City = "Copenhagen" });
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(fakeOpenWeatherClient, repo, weatherServiceConfig, loggerMock);
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert - cancellation should propagate and no measurement should be saved
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => sut.GetAndSaveWeather(cts.Token));
        var measurements = await repo.GetAll(CancellationToken.None);
        Assert.Empty(measurements);
    }

    [Fact]
    public async Task GetMeasurement_FirstCall_ShouldFetchLocation()
    {
        // Arrange
        var (repo, anchor) = CreateRepository();
        using var _ = anchor;
        var openWeatherClientMock = Substitute.For<IOpenWeatherClient>();
        var expectedLocation = new Location("London", 51.5156, -0.0919);
        openWeatherClientMock.GetLocation(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(expectedLocation);
        openWeatherClientMock.GetWeather(Arg.Any<Location>(), Arg.Any<CancellationToken>()).Returns(new TemperatureMeasurement(20.0, expectedLocation.Name, DateTime.UtcNow));
        var weatherServiceConfig = Options.Create(new WeatherServiceConfiguration { City = "London" });
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(openWeatherClientMock, repo, weatherServiceConfig, loggerMock);

        // Act
        await sut.GetAndSaveWeather(CancellationToken.None);

        // Assert
        var measurements = await repo.GetAll(CancellationToken.None);
        Assert.NotEmpty(measurements);
        Assert.Equal(expectedLocation.Name, measurements[0].City);
    }
}
