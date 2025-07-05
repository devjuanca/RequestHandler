using SampleAPI.Dtos;
using SampleAPI.Repository;

namespace SampleAPI.Handlers;

public class AddForecastHandler(WeatherForecastRepository weatherForecast) : RequestHandler<CreateCityForecastCommand, Empty>
{
    public override Task<Empty> HandleAsync(CreateCityForecastCommand request, CancellationToken cancellationToken = default)
    {
        weatherForecast.Add(request.City, request.WeatherForecast);

        return Task.FromResult(Empty.Value);
    }
}
