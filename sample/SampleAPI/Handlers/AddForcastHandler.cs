using SampleAPI.Dtos;
using SampleAPI.Repository;

namespace SampleAPI.Handlers;

public class AddForcastHandler(WeatherForecastRepository weatherForecast) : RequestHandler<AddCityForecastCommand, Empty>
{
    public override Task<Empty> HandleAsync(AddCityForecastCommand request, CancellationToken cancellationToken = default)
    {
        weatherForecast.Add(request.City, request.WeatherForecast);

        return Task.FromResult(Empty.Value);
    }
}
