using EasyRequestHandlers.Common;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyRequestHandlers.Request
{
    /// <summary>
    /// Provides extension methods to register request handlers into the dependency injection container.
    /// </summary>
    public static class HandlersRegister
    {
        /// <summary>
        /// Registers request handlers from the specified assemblies into the dependency injection container with the desired service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to which the request handlers will be added.</param>
        /// <param name="assemblyMarkers">An array of types used to identify the assemblies containing request handlers.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> containing the registered request handlers.</returns>
        public static RequestHandlerBuilder AddEasyRequestHandlers(this IServiceCollection services, params Type[] assemblyMarkers)
        {
            var options = new RequestHandlerOptions();

            return new RequestHandlerBuilder(services, options, assemblyMarkers);
        }

        internal static IServiceCollection RegisterHandlers(IServiceCollection services, RequestHandlerOptions options, Type[] assemblyMarkers)
        {

            var handlers = new List<Type>();

            var handlerKeys = new HashSet<string>();

            foreach (var type in assemblyMarkers)
            {
                var assembly = type.Assembly;

                var foundHandlers = assembly.GetTypes().Where(x => typeof(BaseHandler)
                                                       .IsAssignableFrom(x) &&
                                                        !x.IsAbstract &&
                                                        !x.IsInterface)
                                                       .ToList();

                if (foundHandlers.Count != 0)
                {
                    handlers.AddRange(foundHandlers);
                }

                if (options.EnableRequestHooks)
                {
                    var hookTypes = assembly.GetTypes().Where(x => !x.IsAbstract &&
                            !x.IsInterface &&
                            x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHook<,>))).ToList();

                    foreach (var hookType in hookTypes)
                    {
                        var interfaces = hookType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequestHook<,>));

                        foreach (var hookInterface in interfaces)
                        {
                            services.AddTransient(hookInterface, hookType);
                        }
                    }
                }

            }

            foreach (var handler in handlers)
            {
                var key = GetHandlerKey(handler);

                if (!handlerKeys.Add(key))
                {
                    throw new InvalidOperationException($"Duplicate handler detected for request signature: {key}");
                }

                var baseType = handler.BaseType;

                var lifetimeAttribute = handler.GetCustomAttribute<HandlerLifetimeAttribute>();

                var lifetime = lifetimeAttribute?.Lifetime ?? ServiceLifetime.Transient;

                if (options.EnableMediatorPattern)
                {
                    if (baseType?.IsGenericType == true && baseType.GetGenericTypeDefinition() == typeof(RequestHandler<,>))
                    {
                        services.Add(new ServiceDescriptor(baseType, handler, lifetime));
                    }
                }

                if (options.EnableHandlerInjection)
                {
                    services.Add(new ServiceDescriptor(handler, handler, lifetime));
                }
            }

            if (options.EnableMediatorPattern)
            {
                services.AddScoped<ISender, Sender>();
            }

            services.AddSingleton(options);

            return services;
        }



        private static string GetHandlerKey(Type type)
        {
            var baseType = type.BaseType;

            while (baseType != null && baseType != typeof(BaseHandler))
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == typeof(RequestHandler<,>))
                {
                    var genericArgs = baseType.GetGenericArguments();

                    return $"{baseType.Name}<{genericArgs[0].FullName},{genericArgs[1].FullName}>";
                }

                baseType = baseType.BaseType;
            }
            return type.FullName;
        }

    }
}
