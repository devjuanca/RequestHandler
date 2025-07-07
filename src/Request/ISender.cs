using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        private static readonly ConcurrentDictionary<Type, Func<IServiceProvider, object>> _factoryCache = new ConcurrentDictionary<Type, Func<IServiceProvider, object>>();

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

            var handler = (RequestHandler<TRequest, TResponse>)GetHandler(typeof(RequestHandler<TRequest, TResponse>));

            IPipelineBehaviour<TRequest, TResponse>[] behaviors;
            
            IRequestHook<TRequest, TResponse>[] hooks;
            
            IRequestPreHook<TRequest>[] preHooks;
            
            IRequestPostHook<TRequest, TResponse>[] postHooks;

            behaviors = _serviceProvider.GetServices<IPipelineBehaviour<TRequest, TResponse>>().ToArray();

            if (_options.EnableRequestHooks)
            {
                hooks = _serviceProvider.GetServices<IRequestHook<TRequest, TResponse>>().ToArray();
                preHooks = _serviceProvider.GetServices<IRequestPreHook<TRequest>>().ToArray();
                postHooks = _serviceProvider.GetServices<IRequestPostHook<TRequest, TResponse>>().ToArray();
            }
            else
            {
                hooks = Array.Empty<IRequestHook<TRequest, TResponse>>();
                preHooks = Array.Empty<IRequestPreHook<TRequest>>();
                postHooks = Array.Empty<IRequestPostHook<TRequest, TResponse>>();
            }

            if (behaviors.Length == 0 && hooks.Length == 0 && preHooks.Length == 0 && postHooks.Length == 0)
            {
                return handler.HandleAsync(request, cancellationToken);
            }

            RequestHandlerDelegate<TResponse> handlerDelegate = async () =>
            {
                if (_options.EnableRequestHooks)
                {
                    if (preHooks.Length > 0)
                    {
                        for (int i = 0; i < preHooks.Length; i++)
                        {
                            await preHooks[i].OnExecutingAsync(request, cancellationToken).ConfigureAwait(false);
                        }
                    }

                    if (hooks.Length > 0)
                    {
                        for (int i = 0; i < hooks.Length; i++)
                        {
                            await hooks[i].OnExecutingAsync(request, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                var response = await handler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

                if (_options.EnableRequestHooks)
                {
                    if (postHooks.Length > 0)
                    {
                        for (int i = 0; i < postHooks.Length; i++)
                        {
                            await postHooks[i].OnExecutedAsync(request, response, cancellationToken).ConfigureAwait(false);     
                        }
                    }

                    if (hooks.Length > 0)
                    {
                        for (int i = 0; i < hooks.Length; i++)
                        {
                            await hooks[i].OnExecutedAsync(request, response, cancellationToken).ConfigureAwait(false);
                        }
                    }
                }

                return response;
            };

            if (behaviors.Length > 0)
            {
                for (int i = behaviors.Length - 1; i >= 0; i--)
                {
                    var next = handlerDelegate;
                    var behavior = behaviors[i];
                    handlerDelegate = () => behavior.Handle(request, cancellationToken, next);
                }
            }

            return handlerDelegate();
        }

        public Task<TResponse> SendAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            var handler = _serviceProvider.GetRequiredService<RequestHandler<TResponse>>();

            return handler.HandleAsync(cancellationToken);
        }

        private object GetHandler(Type type)
        {
            var factory = _factoryCache.GetOrAdd(type, CreateFactory);
            return factory(_serviceProvider);
        }

        private static Func<IServiceProvider, object> CreateFactory(Type type)
        {
            var providerParam = Expression.Parameter(typeof(IServiceProvider), "provider");

            var getServiceCall = Expression.Call(
                typeof(ServiceProviderServiceExtensions),
                nameof(ServiceProviderServiceExtensions.GetRequiredService),
                new[] { type },
                providerParam
            );

            var castResult = Expression.Convert(getServiceCall, typeof(object));

            var lambda = Expression.Lambda<Func<IServiceProvider, object>>(castResult, providerParam);

            return lambda.Compile();
        }

    }
}