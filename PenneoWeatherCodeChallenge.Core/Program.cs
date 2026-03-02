using PenneoWeatherCodeChallenge.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IOpenWeatherClient, OpenWeatherClient>();
builder.Services.Configure<OpenWeatherClientConfiguration>(builder.Configuration.GetSection("WeatherServiceConfiguration"));
builder.Services.AddScoped<MeasurementRepository>();

builder.Services.AddScoped<WeatherPollingService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<WeatherPollingService>());
builder.Services.AddScoped<WeatherService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

