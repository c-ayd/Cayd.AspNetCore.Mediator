using Cayd.AspNetCore.Mediator.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Cayd.AspNetCore.Mediator.DependencyInjection
{
    public static class MediatorDependencyInjection
    {
        public static void AddMediator(this IServiceCollection services, Action<MediatorConfig> configure)
        {
            var config = new MediatorConfig(services);
            configure(config);

            foreach (var assembly in config.GetAssemblies())
            {
                AddAllHandlersInAssembly(services, assembly);
            }

            services.AddScoped<IMediator, Mediator>();

            SetAnyMediatorFlowBool(config.IsThereAnyMediatorFlow());
        }

        private static void AddAllHandlersInAssembly(IServiceCollection services, Assembly assembly)
        {
            var allHandlers = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && 
                    t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncHandler<,>)))
                .Select(t => new
                {
                    InterfaceType = t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncHandler<,>))
                        .First(),
                    ImplementationType = t
                })
                .ToList();

            foreach (var handler in allHandlers)
            {
                services.AddScoped(handler.InterfaceType, handler.ImplementationType);
            }
        }

        private static void SetAnyMediatorFlowBool(bool isThereAnyMediatorFlow)
        {
            var fieldInfo = typeof(Mediator).GetField("IsThereAnyMediatorFlow", BindingFlags.NonPublic | BindingFlags.Static)!;
            fieldInfo.SetValue(null, isThereAnyMediatorFlow);
        }
    }
}
