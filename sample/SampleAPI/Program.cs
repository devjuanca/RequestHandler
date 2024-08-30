using SampleAPI.Events;
using SampleAPI.Handlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRequestHandlers(typeof(Program));

builder.Services.RegisterEventsHandlers(typeof(Program));

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


app.MapPost("/notification", async (NotificationEvent @event, IEventPublisher publisher, CancellationToken cancellationToken) =>
{
    await publisher.PublishAsync(@event, useParallelExecution: true, cancellationToken);

    return Results.NoContent();
});

app.Run();

