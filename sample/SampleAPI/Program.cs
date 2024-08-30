using SampleAPI.Dtos;
using SampleAPI.Events;
using SampleAPI.Handlers;
using SampleAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherForecastRepository>();

builder.Services.AddRequestHandlers(typeof(Program));

builder.Services.RegisterEventsHandlers(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();


//REQUEST HANDLERS

app.MapGet("/weatherforecast", async (CitiesForcastHandler handler, CancellationToken cancellationToken) =>
{
    return await handler.HandleAsync(cancellationToken);
});


app.MapGet("/weatherforecast/{city}", async (string city, CityForecastHandler handler, CancellationToken cancellationToken) =>
{
    var result = await handler.HandleAsync(city, cancellationToken);

    return result switch
    {
        not null => Results.Ok(result),
        _ => Results.NotFound()
    };
});


app.MapPost("/weatherforecast", async (AddCityForecastCommand command, AddForcastHandler handler, CancellationToken cancellationToken) =>
{
    await handler.HandleAsync(command, cancellationToken);

    return Results.StatusCode(201);
});



// EVENTS HANDLER

app.MapPost("/notification", async (NotificationEvent @event, IEventPublisher publisher, CancellationToken cancellationToken) =>
{
    await publisher.PublishAsync(@event, useParallelExecution: true, cancellationToken);

    return Results.NoContent();
});

app.Run();

