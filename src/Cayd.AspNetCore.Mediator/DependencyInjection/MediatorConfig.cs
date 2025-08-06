using Cayd.AspNetCore.Mediator.Abstractions;
using Cayd.AspNetCore.Mediator.Exceptions;
using Cayd.AspNetCore.Mediator.Flows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cayd.AspNetCore.Mediator.DependencyInjection
{
    public class MediatorConfig
    {
        private readonly IServiceCollection _services;
        private readonly List<Assembly> _assemblies;
        private bool _isThereAnyMediatorFlow;

        public MediatorConfig(IServiceCollection services)
        {
            _services = services;
            _assemblies = new List<Assembly>();
            _isThereAnyMediatorFlow = false;
        }

        /// <summary>
        /// Finds and adds all classes implementing <see cref="IAsyncHandler{TRequest, TResponse}"/> in a given assembly.
        /// </summary>
        /// <param name="assembly">Assembly from which handlers to add</param>
        public void AddHandlersFromAssembly(Assembly assembly)
            => _assemblies.Add(assembly);

        /// <summary>
        /// Finds and adds all classes implementing <see cref="IAsyncHandler{TRequest, TResponse}"/> in given assemblies.
        /// </summary>
        /// <param name="assemblies">Assemblies from which handlers to add</param>
        public void AddHandlersFromAssemblies(params Assembly[] assemblies)
            => _assemblies.AddRange(assemblies);

        /// <summary>
        /// Adds a custom transient flow element.
        /// </summary>
        /// <param name="flowType">Type of the custom flow element</param>
        public void AddTransientFlow(Type flowType)
            => AddFlow(flowType, ServiceLifetime.Transient);

        /// <summary>
        /// Adds a custom scoped flow element.
        /// </summary>
        /// <param name="flowType">Type of the custom flow element</param>
        public void AddScopedFlow(Type flowType)
            => AddFlow(flowType, ServiceLifetime.Scoped);

        /// <summary>
        /// Adds a custom singleton flow element.
        /// </summary>
        /// <param name="flowType">Type of the custom flow element</param>
        public void AddSingletonFlow(Type flowType)
            => AddFlow(flowType, ServiceLifetime.Singleton);

        private void AddFlow(Type flowType, ServiceLifetime lifetime)
        {
            if (!flowType.IsClass || flowType.IsAbstract)
                throw new WrongTypeException(flowType.Name);

            var implementationType = flowType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMediatorFlow<,>))
                .FirstOrDefault();

            if (implementationType == null)
                throw new MissingInterfaceException(flowType.Name);

            if (flowType.IsGenericTypeDefinition)
            {
                _services.Add(new ServiceDescriptor(typeof(IMediatorFlow<,>), flowType, lifetime));
            }
            else
            {
                _services.Add(new ServiceDescriptor(implementationType, flowType, lifetime));
            }

            _isThereAnyMediatorFlow = true;
        }

        internal List<Assembly> GetAssemblies() => _assemblies;
        internal bool IsThereAnyMediatorFlow() => _isThereAnyMediatorFlow;
    }
}
