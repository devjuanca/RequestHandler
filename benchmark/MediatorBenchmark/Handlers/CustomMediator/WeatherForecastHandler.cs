using CustomMediator;
using MediatorBenchmark.Responses;

namespace MediatorBenchmark.Handlers.CustomMediator;

public class WeatherForecastHandler : RequestHandler<string, WeatherForecast[]>
{
    public readonly (string City, string Weather)[] summaries =
       [
           ("London", "Freezing"),
           ("London", "Bracing"),
           ("London", "Chilly"),
           ("London", "Cool"),
           ("London", "Mild"),
           ("London", "Warm"),
           ( "London", "Balmy"),
           ("London", "Hot"),
           ( "London", "Sweltering"),
           ( "London", "Scorching")
       ];

    public override async ValueTask<WeatherForecast[]> HandleAsync(string cityName, CancellationToken cancellationToken = default)
    {
        var sumaries = summaries.Where(a => a.City.Equals(cityName, StringComparison.OrdinalIgnoreCase))
            .Select(a => a.Weather)
            .ToList();

        if (sumaries.Count == 0)
            return [];

        var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),

            Random.Shared.Next(-20, 55),

            sumaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();

        await Task.CompletedTask; //Simulating async processing.

        return forecast;
    }
}
