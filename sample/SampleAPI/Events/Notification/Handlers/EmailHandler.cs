using EasyRequestHandlers.Events;
using SampleAPI.Events.Notification;

namespace SampleAPI.Events.Notification.Handlers;

public class EmailHandler(ILogger<EmailHandler> logger) : IEventHandler<NotificationEvent>
{
    public async Task HandleAsync(NotificationEvent @event, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        logger.LogInformation("EMAIL sent. EventId: {id}. Message: {message}", @event.Id, @event.Message);
    }
}
