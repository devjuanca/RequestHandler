
using SampleAPI.Dtos;

namespace SampleAPI.Hooks;

public class CreateCityForecastAlertHook(ILogger<CreateCityForecastAlertHook> logger) : IRequestHook<CreateCityForecastCommand, Empty>
{
    public Task OnExecutingAsync(CreateCityForecastCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Hook Executing. New forecast for city: {city}", request.City);

        if (request.WeatherForecast.TemperatureC < 0)
        {
            logger.LogWarning("It is gonna be cold!!!");
        }

        if (request.WeatherForecast.TemperatureC > 30)
        {
            logger.LogWarning("It is gonna be hot!!!");
        }

        return Task.CompletedTask;
    }

    public Task OnExecutedAsync(CreateCityForecastCommand request, Empty response, CancellationToken cancellationToken)
    {
        logger.LogInformation("Hook Executed. New forecast for city: {city}", request.City);

        return Task.CompletedTask;
    }
}

public class CreateCityForecastPreHook(ILogger<CreateCityForecastPreHook> logger) : IRequestPreHook<CreateCityForecastCommand>
{
    public Task OnExecutingAsync(CreateCityForecastCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Pre Hook Executing. New forecast for city: {city}", request.City);

        request.City = request.City.Trim();

        return Task.CompletedTask;
    }
}

public class CreateCityForecastPostHook(ILogger<CreateCityForecastPostHook> logger) : IRequestPostHook<CreateCityForecastCommand, Empty>
{
    public Task OnExecutedAsync(CreateCityForecastCommand request, Empty response, CancellationToken cancellationToken)
    {
        logger.LogInformation("Post Hook Executed. New forecast for city: {city}", request.City);
        return Task.CompletedTask;
    }
}
