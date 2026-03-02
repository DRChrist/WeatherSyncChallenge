using System.Text.Json.Serialization;

namespace PenneoWeatherCodeChallenge.Core;

public record MainDto(
    [property: JsonPropertyName("temp")] double Temp);