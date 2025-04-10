using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EasyRequestHandlers.Request
{
    public interface ISender
    {
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default);

        Task<TResponse> SendAsync<TResponse>(CancellationToken cancellationToken = default);
    }

    public class Sender : ISender
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly RequestHandlerOptions _options;

        public Sender(IServiceProvider serviceProvider, RequestHandlerOptions options)
        {
            _serviceProvider = serviceProvider;

            _options = options;
        }

        public Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var handler = _serviceProvider.GetRequiredService<RequestHandler<TRequest, TResponse>>();

            var behaviors = _serviceProvider.GetServices<IPipelineBehaviour<TRequest, TResponse>>()
                                            .Reverse()
                                            .ToList();
            List<IRequestHook<TRequest, TResponse>> hooks = new List<IRequestHook<TRequest, TResponse>>();

            if (_options.EnableRequestHooks)
            {
                hooks = _serviceProvider.GetServices<IRequestHook<TRequest, TResponse>>().ToList();
            }

            RequestHandlerDelegate<TResponse> handlerDelegate = async () =>
            {
                if (_options.EnableRequestHooks)
                {
                    foreach (var hook in hooks)
                    {
                        await hook.OnExecutingAsync(request, cancellationToken);
                    }
                }

                var response = await handler.HandleAsync(request, cancellationToken);

                if (_options.EnableRequestHooks)
                {
                    foreach (var hook in hooks)
                    {
                        await hook.OnExecutedAsync(request, response, cancellationToken);
                    }
                }

                return response;
            };

            foreach (var behavior in behaviors)
            {
                var next = handlerDelegate;

                handlerDelegate = () => behavior.Handle(request, cancellationToken, next);
            }

            return handlerDelegate();
        }

        public Task<TResponse> SendAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            var handler = _serviceProvider.GetRequiredService<RequestHandler<TResponse>>();

            return handler.HandleAsync(cancellationToken);
        }
    }
}