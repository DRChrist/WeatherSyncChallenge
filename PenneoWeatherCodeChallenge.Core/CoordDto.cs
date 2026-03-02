using System.Text.Json.Serialization;

namespace PenneoWeatherCodeChallenge.Core;

public record CoordDto(
    [property: JsonPropertyName("lat")] double Lat,
    [property: JsonPropertyName("lon")] double Lon);