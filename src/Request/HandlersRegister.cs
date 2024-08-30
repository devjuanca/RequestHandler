using Microsoft.Extensions.DependencyInjection;
using RequestHandlers.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EasyRequestHandler.Request
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
        /// <param name="assemblyTypes">An array of types used to identify the assemblies containing request handlers.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> containing the registered request handlers.</returns>
        public static IServiceCollection AddRequestHandlers(this IServiceCollection services, params Type[] assemblyTypes)
        {
            var handlers = new List<Type>();

            foreach (var type in assemblyTypes)
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
            }

            foreach (var handler in handlers)
            {
                var lifetimeAttribute = handler.GetCustomAttribute<HandlerLifetimeAttribute>();

                if (lifetimeAttribute == null)
                {
                    services.AddTransient(handler);
                    continue;
                }

                switch (lifetimeAttribute.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        services.AddSingleton(handler);
                        break;
                    case ServiceLifetime.Scoped:
                        services.AddScoped(handler);
                        break;
                    case ServiceLifetime.Transient:
                        services.AddTransient(handler);
                        break;
                }
            }
            return services;
        }
    }
}
