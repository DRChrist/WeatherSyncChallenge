namespace PenneoWeatherCodeChallenge.Core;

public record TemperatureMeasurement(double Temperature, // In Celsius
                                     Location Location,
                                     DateTime Timestamp);