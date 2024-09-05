using Microsoft.Extensions.DependencyInjection;
using System;

namespace EasyRequestHandlers.Common
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
