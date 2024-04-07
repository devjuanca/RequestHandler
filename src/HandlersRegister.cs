using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CustomMediator
{
    public static class HandlersRegister
    {
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
