using MediatorBenchmark.Responses;
using MediatR;
namespace MediatorBenchmark.Handlers;

public record CityRequest : IRequest<WeatherForecast[]>
{
    public required string CityName { get; set; }
}

public class WeatherForecastMediatrHandler : IRequestHandler<CityRequest, WeatherForecast[]>
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

    public async Task<WeatherForecast[]> Handle(CityRequest cityRequest, CancellationToken cancellationToken)
    {
        var cityName = cityRequest.CityName;

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
