namespace SampleAPI.Events.CreateCityForcastEvent;

public class CreateCityForcastEvent
{
    public required string City { get; set; }

    public DateTime TimeSpan { get; set; }
}
