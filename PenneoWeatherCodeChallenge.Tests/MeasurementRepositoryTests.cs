using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;
using PenneoWeatherCodeChallenge.Core;

namespace PenneoWeatherCodeChallenge.Tests;

public class MeasurementRepositoryTests
{
    [Fact]
    public async Task SaveMeasurement_ShouldSaveMeasurement()
    {
        // Arrange
        var connectionString = $"Data Source={Guid.NewGuid()};Mode=Memory;Cache=Shared";
        using var anchor = new SqliteConnection(connectionString);
        await anchor.OpenAsync();

        var configuration = Options.Create(new MeasurementRepositoryConfiguration { ConnectionString = connectionString });
        var sut = new MeasurementRepository(configuration);
        sut.InitializeDatabase();
        var measurement = new TemperatureMeasurement(25.0, "TestCity", DateTime.UtcNow);

        // Act
        await sut.Add(measurement, CancellationToken.None);

        // Assert
        var result = await sut.GetAll(CancellationToken.None);
        Assert.Single(result);
        Assert.Equal(measurement.Temperature, result[0].Temperature);
    }
}