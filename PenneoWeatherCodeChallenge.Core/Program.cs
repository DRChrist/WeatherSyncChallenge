using PenneoWeatherCodeChallenge.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IOpenWeatherClient, OpenWeatherClient>();
builder.Services.AddSingleton<MeasurementRepository>();
builder.Services.AddSingleton<WeatherPollingService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<WeatherPollingService>());
builder.Services.AddSingleton<WeatherService>();

builder.Services.Configure<OpenWeatherClientConfiguration>(builder.Configuration.GetSection("WeatherServiceConfiguration"));
builder.Services.Configure<WeatherServiceConfiguration>(builder.Configuration.GetSection("WeatherServiceConfiguration"));
builder.Services.Configure<MeasurementRepositoryConfiguration>(builder.Configuration.GetSection("MeasurementRepositoryConfiguration"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

