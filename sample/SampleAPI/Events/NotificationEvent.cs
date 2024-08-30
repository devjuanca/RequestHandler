namespace SampleAPI.Events;

public class NotificationEvent
{
    public Guid Id { get; } = Guid.NewGuid();


    public string? Message { get; set; }
}
