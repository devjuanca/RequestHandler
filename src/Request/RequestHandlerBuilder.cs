using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Linq;

namespace EasyRequestHandlers.Request
{
    public class RequestHandlerBuilder
    {
        private readonly IServiceCollection _services;

        private readonly RequestHandlerOptions _options;

        private readonly Type[] _assemblyMarkers;

        public RequestHandlerBuilder(IServiceCollection services, RequestHandlerOptions options, Type[] assemblyMarkers)
        {
            _services = services;

            _options = options;

            _assemblyMarkers = assemblyMarkers;
        }

        public RequestHandlerBuilder WithMediatorPattern()
        {
            _options.EnableMediatorPattern = true;

            return this;
        }

        public RequestHandlerBuilder RemoveHandlerInjection()
        {
            _options.EnableHandlerInjection = false;

            return this;
        }

        public RequestHandlerBuilder WithBehaviour(Type openGenericBehaviour)
        {
            if (!_options.EnableMediatorPattern)
            {
                return this;
            }

            if (!openGenericBehaviour.IsGenericTypeDefinition)
                throw new ArgumentException("Type must be an open generic like typeof(MyBehaviour<,>)");

            var implementsInterface = openGenericBehaviour.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehaviour<,>));

            if (!implementsInterface)
                throw new ArgumentException($"Type {openGenericBehaviour.Name} must implement IPipelineBehaviour<,>");

            _services.TryAddEnumerable(
                ServiceDescriptor.Transient(typeof(IPipelineBehaviour<,>), openGenericBehaviour));

            return this;
        }

        public RequestHandlerBuilder WithBehaviours(params Type[] behaviourTypes)
        {
            if (!_options.EnableMediatorPattern)
            {
                return this;
            }

            foreach (var behaviourType in behaviourTypes)
            {
                if (!behaviourType.IsGenericTypeDefinition || behaviourType.GetGenericTypeDefinition() != behaviourType)
                {
                    throw new ArgumentException($"Behaviour type {behaviourType.Name} must be an open generic type, e.g. LoggingBehaviour<,>");
                }

                var implementsInterface = behaviourType.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPipelineBehaviour<,>));

                if (!implementsInterface)
                {
                    throw new ArgumentException($"Type {behaviourType.Name} must implement IPipelineBehaviour<,>");
                }

                _services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IPipelineBehaviour<,>), behaviourType));
            }

            return this;
        }

        public RequestHandlerBuilder WithRequestHooks()
        {
            _options.EnableRequestHooks = true;

            return this;
        }


        public IServiceCollection Build()
        {
            HandlersRegister.RegisterHandlers(_services, _options, _assemblyMarkers);

            return _services;
        }
    }

}