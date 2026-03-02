using PenneoWeatherCodeChallenge.Core;
using NSubstitute;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace PenneoWeatherCodeChallenge.Tests;

public class WeatherServiceTests
{
    [Fact]
    public async Task GetMeasurement_HappyPath()
    {
        // Arrange
        var fakeOpenWeatherClient = new FakeOpenWeatherClient();
        var measurementRepositoryConfig = new MeasurementRepositoryConfiguration("Data Source=GetMeasurement_HappyPath.db");
        var fakeMeasurementRepository = new MeasurementRepository(measurementRepositoryConfig);
        fakeMeasurementRepository.InitializeDatabase();
        fakeMeasurementRepository.ClearMeasurements(); // Ensure the database is empty before the test
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var weatherServiceConfig = new WeatherServiceConfiguration { City = "Copenhagen" };
        var sut = new WeatherService(fakeOpenWeatherClient, fakeMeasurementRepository, weatherServiceConfig, loggerMock);

        // Act
        await sut.GetAndSaveWeather(CancellationToken.None);

        // Assert
        var measurements = await fakeMeasurementRepository.GetAllMeasurements(CancellationToken.None);
        Assert.NotEmpty(measurements);
    }

    [Fact]
    public async Task GetMeasurement_WeatherServiceThrows_ShouldLogError()
    {
        // Arrange
        var openWeatherClientMock = Substitute.For<IOpenWeatherClient>();
        openWeatherClientMock.GetWeather(Arg.Any<Location>(), Arg.Any<CancellationToken>()).Throws(new Exception("API error"));
        var measurementRepositoryConfig = new MeasurementRepositoryConfiguration("Data Source=GetMeasurement_WeatherServiceThrows_ShouldLogError.db");
        var fakeMeasurementRepository = new MeasurementRepository(measurementRepositoryConfig);
        fakeMeasurementRepository.InitializeDatabase();
        fakeMeasurementRepository.ClearMeasurements(); // Ensure the database is empty before the test
        var weatherServiceConfig = new WeatherServiceConfiguration { City = "Copenhagen" };
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(openWeatherClientMock, fakeMeasurementRepository, weatherServiceConfig, loggerMock);
        var cts = new CancellationTokenSource();

        // Act
        await sut.GetAndSaveWeather(cts.Token);

        // Assert
        loggerMock.Received().LogError("Failed to fetch weather data");
        var measurements = await fakeMeasurementRepository.GetAllMeasurements(CancellationToken.None);
        Assert.Empty(measurements); // No measurements should be saved when the service fails
    }

    [Fact]
    public async Task GetMeasurement_CancellationRequested_ShouldNotSaveMeasurement()
    {
        // Arrange
        var fakeOpenWeatherClient = new FakeOpenWeatherClient();
        var measurementRepositoryConfig = new MeasurementRepositoryConfiguration("Data Source=GetMeasurement_CancellationRequested_ShouldNotSaveMeasurement.db");
        var fakeMeasurementRepository = new MeasurementRepository(measurementRepositoryConfig);
        fakeMeasurementRepository.InitializeDatabase();
        fakeMeasurementRepository.ClearMeasurements(); // Ensure the database is empty before the test
        var weatherServiceConfig = new WeatherServiceConfiguration { City = "Copenhagen" };
        var loggerMock = Substitute.For<ILogger<WeatherService>>();
        var sut = new WeatherService(fakeOpenWeatherClient, fakeMeasurementRepository, weatherServiceConfig, loggerMock);
        var cts = new CancellationTokenSource();
        cts.Cancel(); // Cancel the token before calling the method to simulate cancellation

        // Act
        await sut.GetAndSaveWeather(cts.Token);

        // Assert
        var measurements = await fakeMeasurementRepository.GetAllMeasurements(CancellationToken.None);
        Assert.Empty(measurements); // No measurements should be saved when cancellation is requested
    }
}
