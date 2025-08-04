using System.Collections.Generic;
using System.Reflection;

namespace Cayd.AspNetCore.Mediator.DependencyInjection
{
    public class MediatorConfig
    {
        private List<Assembly> _assemblies = new List<Assembly>();

        public MediatorConfig RegisterHandlersFromAssembly(Assembly assembly)
        {
            _assemblies.Add(assembly);
            return this;
        }

        public MediatorConfig RegisterHandlersFromAssemblies(params Assembly[] assemblies)
        {
            _assemblies.AddRange(assemblies);
            return this;
        }

        public List<Assembly> GetAssemblies() => _assemblies;
    }
}
