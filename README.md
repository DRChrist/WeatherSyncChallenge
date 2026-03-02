# PenneoWeatherCodeChallenge

A .NET 10 background service that periodically fetches temperature data from the [OpenWeatherMap API](https://openweathermap.org/api) and stores it in a local SQLite database. A minimal HTTP API exposes the recorded measurements.

## How it works

1. On startup the app resolves the configured city name to coordinates via the OpenWeatherMap Geocoding API.
2. A background service polls the current temperature every 60 seconds and saves each reading to SQLite.
3. A GET endpoint lets you query the stored measurements.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- An [OpenWeatherMap API key](https://home.openweathermap.org/api_keys) (free tier is sufficient)

## Configuration

The following settings are required. The recommended approach is to use [user secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) so the API key stays out of source control:

```sh
dotnet user-secrets set "OpenWeatherClientConfiguration:ApiKey" "<your-api-key>" --project PenneoWeatherCodeChallenge.Core
```

The remaining settings can be left as defaults in `appsettings.Development.json` or overridden the same way:

| Section | Key | Default | Description |
|---|---|---|---|
| `WeatherServiceConfiguration` | `City` | `Bangkok` | City to fetch weather for |
| `MeasurementRepositoryConfiguration` | `ConnectionString` | `Data Source=weather_measurements.db` | SQLite connection string |

## Running

```sh
dotnet run --project PenneoWeatherCodeChallenge.Core
```

## API

### `GET /measurements`

Returns the most recent temperature measurements.

**Query parameters**

| Parameter | Type | Default | Description |
|---|---|---|---|
| `limit` | int | `100` | Maximum number of results |
| `from` | datetime | — | Start of date range (requires `to`) |
| `to` | datetime | — | End of date range (requires `from`) |

**Example response**

```json
[
  {
    "temperature": 31.4,
    "city": "Bangkok",
    "timestamp": "2024-03-01T10:00:00"
  }
]
```

The OpenAPI document is available at `/openapi/v1.json` when running in the `Development` environment.

## Running tests

```sh
dotnet test
```
