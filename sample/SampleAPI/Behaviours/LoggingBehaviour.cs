using System.Diagnostics;

namespace SampleAPI.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) : IPipelineBehaviour<TRequest, TResponse>
{
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var startTime = Stopwatch.GetTimestamp();

        try
        {
            logger.LogInformation("Behaviour: Logging request: {Request}", request);

            return next();

        }
        finally
        {
            var elapsedTime = Stopwatch.GetElapsedTime(startTime);

            logger.LogInformation("Behaviour: Finished logging request: {Request}. Duration: {duration}", request, elapsedTime);
        }
    }
}
