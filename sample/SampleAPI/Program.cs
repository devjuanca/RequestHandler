using CustomMediator;
using SampleAPI.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestHandlers(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();



app.MapGet("/weatherforecast", async (WeatherHandler handler, CancellationToken cancellationToken) =>
{
    return await handler.HandleAsync(cancellationToken);
});

app.MapGet("/weatherforecast/{city}", async (string city, CityWeatherForecastHandler handler, CancellationToken cancellationToken) =>
{
    return await handler.HandleAsync(city, cancellationToken);
});

app.Run();

