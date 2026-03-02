using Microsoft.Data.Sqlite;

namespace PenneoWeatherCodeChallenge.Core;

public class MeasurementRepository(MeasurementRepositoryConfiguration configuration)
{
    public async Task SaveMeasurement(TemperatureMeasurement measurement, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(configuration.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO TemperatureMeasurements (Temperature, Location, Timestamp)
            VALUES ($temperature, $location, $timestamp)
        ";
        command.Parameters.AddWithValue("$temperature", measurement.Temperature);
        command.Parameters.AddWithValue("$location", measurement.Location.Name);
        command.Parameters.AddWithValue("$timestamp", measurement.Timestamp);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}