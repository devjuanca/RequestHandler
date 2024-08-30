using RequestHandlers.Common;
using RequestHandlers.Events.Services;
namespace SampleAPI.Events.Handlers;

[HandlerLifetime(ServiceLifetime.Singleton)]
public class SmsHandler(ILogger<SmsHandler> logger) : IEventHandler<NotificationEvent>
{
    public async Task HandleAsync(NotificationEvent @event, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        logger.LogInformation("SMS sent. EventId: {id}. Message: {message}", @event.Id, @event.Message);
    }
}
