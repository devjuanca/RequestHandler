using SampleAPI.Behaviours;
using SampleAPI.Dtos;
using SampleAPI.Events.CreateCityForcastEvent;
using SampleAPI.Events.Notification;
using SampleAPI.Handlers;
using SampleAPI.Repository;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<WeatherForecastRepository>();

builder.Services.AddEasyRequestHandlers(typeof(Program))
                .WithMediatorPattern()
                .WithBehaviours(
                    typeof(LoggingBehaviour<,>),
                    typeof(AuthenticationBehaviour<,>),
                    typeof(ValidationBehaviour<,>)
                    )
                .WithRequestHooks()
                .Build();


builder.Services.AddEasyEventHandlers(typeof(Program));

var app = builder.Build();

app.UseHttpsRedirection();

//REQUEST HANDLERS

app.MapGet("/weatherforecast", async (CitiesForcastHandler handler, CancellationToken cancellationToken) =>
{
    return await handler.HandleAsync(cancellationToken);
});

app.MapGet("/weatherforecast/{city}", async (string city, ISender sender, CancellationToken cancellationToken) =>
{
    var result = await sender.SendAsync<string, WeatherForecast?>(city, cancellationToken: cancellationToken);

    return result switch
    {
        not null => Results.Ok(result),
        _ => Results.NotFound()
    };
});

app.MapPost("/weatherforecast", async (CreateCityForecastCommand command, ISender sender, IEventPublisher publisher, CancellationToken cancellationToken) =>
{
    await sender.SendAsync<CreateCityForecastCommand, Empty>(command, cancellationToken);

    await publisher.PublishAsync(new CreateCityForcastEvent
    {
        City = command.City
    }, cancellationToken: cancellationToken);

    return Results.StatusCode(201);
});

// EVENTS HANDLER

app.MapPost("/notification", async (NotificationEvent @event, IEventPublisher publisher, CancellationToken cancellationToken) =>
{
    await publisher.PublishAsync(@event, useParallelExecution: true, cancellationToken);

    return Results.NoContent();
});

app.Run();