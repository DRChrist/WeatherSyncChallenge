using Microsoft.Extensions.Options;
using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class MeasurementRepositoryTests
{
    [Fact]
    public async Task SaveMeasurement_ShouldSaveMeasurement()
    {
        // Arrange
        var configuration = Options.Create<MeasurementRepositoryConfiguration>(new MeasurementRepositoryConfiguration { ConnectionString ="Data Source=measurements.db" });
        var sut = new MeasurementRepository(configuration);
        sut.InitializeDatabase();
        sut.ClearMeasurements();
        var measurement = new TemperatureMeasurement(25.0, new Location("TestCity", 10.0, 20.0), DateTime.UtcNow);

        // Act
        await sut.SaveMeasurement(measurement, CancellationToken.None);

        // Assert
        var result = await sut.GetAllMeasurements(CancellationToken.None);
        Assert.Single(result);
        Assert.Equal(measurement.Temperature, result[0].Temperature);
    }
}