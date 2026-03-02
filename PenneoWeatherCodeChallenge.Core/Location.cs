using System.Text.Json.Serialization;

namespace PenneoWeatherCodeChallenge.Core;

public record Location(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("lat")] double Latitude,
    [property: JsonPropertyName("lon")] double Longitude);