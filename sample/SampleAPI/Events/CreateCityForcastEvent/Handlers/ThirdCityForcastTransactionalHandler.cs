
namespace SampleAPI.Events.CreateCityForcastEvent.Handlers;

public class ThirdCityForcastTransactionalHandler(ILogger<ThirdCityForcastTransactionalHandler> logger) : ITransactionalEventHandler<CreateCityForcastEvent>
{
    public int Order { get; } = 2;

    public Task HandleAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Executing Third Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task CommitAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Commit Third Second Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }

    public Task RollbackAsync(CreateCityForcastEvent @event, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rolling Back Failed Third Transactional Handler for Create City Forcast Event");

        return Task.CompletedTask;
    }
}
