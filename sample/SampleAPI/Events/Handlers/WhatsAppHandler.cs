using RequestHandlers.Events.Services;

namespace SampleAPI.Events.Handlers;

public class WhatsAppHandler(ILogger<WhatsAppHandler> logger) : IEventHandler<NotificationEvent>
{
    public async Task HandleAsync(NotificationEvent @event, CancellationToken cancellationToken)
    {
        await Task.Delay(1000, cancellationToken);

        logger.LogInformation("WHATSAPP sent. EventId: {id}. Message: {message}", @event.Id, @event.Message);
    }
}
