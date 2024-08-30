using SampleAPI.Dtos;
using SampleAPI.Repository;

namespace SampleAPI.Handlers;

public class CityForecastHandler(WeatherForecastRepository forecastRepository) : RequestHandler<string, WeatherForecast?>
{
    public override Task<WeatherForecast?> HandleAsync(string cityName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(forecastRepository.Get(cityName));
    }
}
