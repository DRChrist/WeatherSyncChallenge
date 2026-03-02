namespace PenneoWeatherCodeChallenge.Core;

public record TemperatureMeasurement(double Temperature, // In Celsius
                                     string City,
                                     DateTime Timestamp);