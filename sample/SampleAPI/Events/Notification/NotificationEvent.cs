namespace SampleAPI.Events.Notification;

public class NotificationEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public string? Message { get; set; }
}
