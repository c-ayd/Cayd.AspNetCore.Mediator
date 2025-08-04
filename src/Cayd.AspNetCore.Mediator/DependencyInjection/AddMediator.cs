using Cayd.AspNetCore.Mediator.Abstraction;
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
            var config = new MediatorConfig();
            configure(config);

            foreach (var assembly in config.GetAssemblies())
            {
                AddAllHandlersInAssembly(services, assembly);
            }

            services.AddScoped<IMediator, Mediator>();
        }

        private static void AddAllHandlersInAssembly(IServiceCollection services, Assembly assembly)
        {
            var allHandlers = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && 
                    t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>)))
                .Select(t => new
                {
                    InterfaceType = t.GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncRequestHandler<,>))
                        .First(),
                    ImplementationType = t
                })
                .ToList();

            foreach (var handler in allHandlers)
            {
                services.AddScoped(handler.InterfaceType, handler.ImplementationType);
            }
        }
    }
}
