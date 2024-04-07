using Microsoft.Extensions.DependencyInjection;
using System;

namespace CustomMediator
{

    [AttributeUsage(AttributeTargets.Class)]
    internal class HandlerLifetimeAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public HandlerLifetimeAttribute(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
        }
    }
}
