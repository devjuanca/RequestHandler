
namespace SampleAPI.Events.CreateCityForcastEvent.Handlers;

public class FirstCityForcastTransactionalHandler(ILogger<FirstCityForcastTransactionalHandler> logger) : ITransactionalEventHandler<CreateCityForcastEvent>
{
    public int Order { get; } = 0;

    public Task HandleAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing First Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task CommitAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Commit Successfull First Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task RollbackAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rolling Back Failed First Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }
}
