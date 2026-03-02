using System.Text.Json.Serialization;

namespace PenneoWeatherCodeChallenge.Core;

public record OpenWeatherResponse(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("coord")] CoordDto Coord,
    [property: JsonPropertyName("main")] MainDto Main,
    [property: JsonPropertyName("dt")] long Dt)
{
    public TemperatureMeasurement ToMeasurement() => new(
        Temperature: Main.Temp,
        Location: new Location(Name, Coord.Lat, Coord.Lon),
        Timestamp: DateTimeOffset.FromUnixTimeSeconds(Dt).UtcDateTime); // TODO: check that this conversion is correct.
}
