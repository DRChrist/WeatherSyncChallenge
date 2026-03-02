using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace PenneoWeatherCodeChallenge.Core;

public class MeasurementRepository(IOptions<MeasurementRepositoryConfiguration> configuration)
{
    public async Task Add(TemperatureMeasurement measurement, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            INSERT INTO TemperatureMeasurements (Temperature, Location, Timestamp)
            VALUES ($temperature, $location, $timestamp)
        ";
        command.Parameters.AddWithValue("$temperature", measurement.Temperature);
        command.Parameters.AddWithValue("$location", measurement.City);
        command.Parameters.AddWithValue("$timestamp", measurement.Timestamp);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<List<TemperatureMeasurement>> GetAll(CancellationToken cancellationToken, int limit = 100)
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT Temperature, Location, Timestamp
            FROM TemperatureMeasurements
            ORDER BY Timestamp DESC
            LIMIT $limit
        ";
        command.Parameters.AddWithValue("$limit", limit);

        var measurements = new List<TemperatureMeasurement>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            measurements.Add(new TemperatureMeasurement(
                reader.GetDouble(0),
                reader.GetString(1),
                reader.GetDateTime(2)
            ));
        }

        return measurements;
    }

    public async Task<List<TemperatureMeasurement>> GetHistory(DateTime from, DateTime to, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT Temperature, Location, Timestamp
            FROM TemperatureMeasurements
            WHERE Timestamp BETWEEN $from AND $to
        ";
        command.Parameters.AddWithValue("$from", from);
        command.Parameters.AddWithValue("$to", to);

        var measurements = new List<TemperatureMeasurement>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            measurements.Add(new TemperatureMeasurement(
                reader.GetDouble(0),
                reader.GetString(1),
                reader.GetDateTime(2)
            ));
        }

        return measurements;
    }

    public void InitializeDatabase()
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            CREATE TABLE IF NOT EXISTS TemperatureMeasurements (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                Temperature REAL NOT NULL,
                Location TEXT NOT NULL,
                Timestamp TEXT NOT NULL
            )
        ";
        command.ExecuteNonQuery();
    }
}