using SampleAPI.Responses;

namespace SampleAPI.Handlers;

public class WeatherHandler : RequestHandler<WeatherForecast[]>
{
    public readonly string[] summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

    public override async ValueTask<WeatherForecast[]> HandleAsync(CancellationToken cancellationToken = default)
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),

            Random.Shared.Next(-20, 55),

            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

        await Task.CompletedTask; //Simulating async processing.

        return forecast;
    }
}
