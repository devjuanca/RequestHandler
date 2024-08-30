using SampleAPI.Dtos;

namespace SampleAPI.Repository;

public class WeatherForecastRepository
{
    private Dictionary<string, WeatherForecast> CitiesForcast { get; } = [];

    public readonly string[] summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    public WeatherForecastRepository()
    {
        CitiesForcast = new Dictionary<string, WeatherForecast>()
        {
            {"London", new WeatherForecast( DateOnly.FromDateTime(DateTime.UtcNow), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]) },
            {"Madrid", new WeatherForecast( DateOnly.FromDateTime(DateTime.UtcNow), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]) },
            {"Paris", new WeatherForecast( DateOnly.FromDateTime(DateTime.UtcNow), Random.Shared.Next(-20, 55), summaries[Random.Shared.Next(summaries.Length)]) }
        };
    }

    public bool Add(string city, WeatherForecast forecast)
    {
        return CitiesForcast.TryAdd(city, forecast);
    }

    public void Update(string city, WeatherForecast forecast)
    {
        CitiesForcast[city] = forecast;
    }

    public WeatherForecast? Get(string city)
    {
        return CitiesForcast.TryGetValue(city, out var forcast) ? forcast : null;
    }

    public Dictionary<string, WeatherForecast> GetAll()
    {
        return CitiesForcast;
    }
}
