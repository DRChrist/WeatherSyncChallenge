using PenneoWeatherCodeChallenge.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.Configure<WeatherServiceConfiguration>(builder.Configuration.GetSection("WeatherServiceConfiguration"));
builder.Services.AddScoped<MeasurementRepository>();

builder.Services.AddScoped<WeatherPollingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

