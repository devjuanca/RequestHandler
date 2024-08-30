using SampleAPI.Dtos;
using SampleAPI.Repository;

namespace SampleAPI.Handlers;

public class CitiesForcastHandler(WeatherForecastRepository forecastRepository) : RequestHandler<Dictionary<string, WeatherForecast>>
{
    public override Task<Dictionary<string, WeatherForecast>> HandleAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(forecastRepository.GetAll());
    }
}
