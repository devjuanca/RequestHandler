using Microsoft.Extensions.DependencyInjection;
using System;

namespace RequestHandlers.Common
{

    [AttributeUsage(AttributeTargets.Class)]
    public class HandlerLifetimeAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public HandlerLifetimeAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
