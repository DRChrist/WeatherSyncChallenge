using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace PenneoWeatherCodeChallenge.Core;

public class MeasurementRepository(IOptions<MeasurementRepositoryConfiguration> configuration)
{
    public async Task SaveMeasurement(TemperatureMeasurement measurement, CancellationToken cancellationToken)
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
        command.Parameters.AddWithValue("$location", measurement.Location.Name);
        command.Parameters.AddWithValue("$timestamp", measurement.Timestamp);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<List<TemperatureMeasurement>> GetAllMeasurements(CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            SELECT Temperature, Location, Timestamp
            FROM TemperatureMeasurements
        ";

        var measurements = new List<TemperatureMeasurement>();
        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            measurements.Add(new TemperatureMeasurement(
                reader.GetDouble(0),
                new Location(reader.GetString(1), 0, 0), // TODO: Location details are not stored in the database, so we use a placeholder
                reader.GetDateTime(2)
            ));
        }

        return measurements;
    }

    public async Task<List<TemperatureMeasurement>> GetMeasurements(DateTime from, DateTime to, CancellationToken cancellationToken)
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
                new Location(reader.GetString(1), 0, 0), // TODO: Location details are not stored in the database, so we use a placeholder
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

    public void ClearMeasurements()
    {
        using var connection = new SqliteConnection(configuration.Value.ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText =
        @"
            DELETE FROM TemperatureMeasurements
        ";
        command.ExecuteNonQuery();
    }
}