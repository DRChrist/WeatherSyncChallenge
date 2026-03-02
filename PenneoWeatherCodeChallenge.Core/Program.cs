using PenneoWeatherCodeChallenge.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IOpenWeatherClient, OpenWeatherClient>(client =>
{
    client.BaseAddress = new Uri("https://api.openweathermap.org/");
    client.Timeout = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<MeasurementRepository>();
builder.Services.AddSingleton<WeatherPollingService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<WeatherPollingService>());
builder.Services.AddSingleton<WeatherService>();

builder.Services.Configure<OpenWeatherClientConfiguration>(builder.Configuration.GetSection("OpenWeatherClientConfiguration"));
builder.Services.Configure<WeatherServiceConfiguration>(builder.Configuration.GetSection("WeatherServiceConfiguration"));
builder.Services.Configure<MeasurementRepositoryConfiguration>(builder.Configuration.GetSection("MeasurementRepositoryConfiguration"));

var app = builder.Build();

app.Services.GetRequiredService<MeasurementRepository>().InitializeDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/measurements", async (MeasurementRepository repo, CancellationToken ct,
    DateTime? from, DateTime? to, int limit = 100) =>
{
    var measurements = (from, to) switch
    {
        ({ } f, { } t) => await repo.GetHistory(f, t, ct),
        _ => await repo.GetAll(ct, limit)
    };

    return Results.Ok(measurements.Select(m => new
    {
        m.Temperature,
        m.City,
        m.Timestamp
    }));
})
.WithName("GetMeasurements")
.WithSummary("Get temperature measurements, optionally filtered by date range");

app.Run();

