
using SampleAPI.Dtos;

namespace SampleAPI.Behaviours;

public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehaviour<TRequest, TResponse>
{
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        if (request is CreateCityForecastCommand and { City: "" or null })
        {
            throw new ArgumentException("City name cannot be empty");
        }

        return next();
    }
}
