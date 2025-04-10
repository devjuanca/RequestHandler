
namespace SampleAPI.Behaviours;

public class AuthenticationBehaviour<TRequest, TResponse>(ILogger<AuthenticationBehaviour<TRequest, TResponse>> logger) : IPipelineBehaviour<TRequest, TResponse>
{
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        logger.LogInformation("Behaviour: Authenticating request: {Request}", request);

        return next();
    }
}
