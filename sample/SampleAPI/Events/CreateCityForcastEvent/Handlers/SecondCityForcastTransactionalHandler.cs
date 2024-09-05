
namespace SampleAPI.Events.CreateCityForcastEvent.Handlers;

public class SecondCityForcastTransactionalHandler(ILogger<SecondCityForcastTransactionalHandler> logger) : ITransactionalEventHandler<CreateCityForcastEvent>
{
    public int Order { get; } = 1;

    public Task HandleAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing Second Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task CommitAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Commit Successfull Second Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task RollbackAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rolling Back Failed Second Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }
}
