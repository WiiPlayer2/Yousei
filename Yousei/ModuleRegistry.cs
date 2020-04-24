using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Yousei.Modules;

namespace Yousei
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    sealed class ModuleIdAttribute : Attribute
    {
        public ModuleIdAttribute(string moduleId)
        {
            Id = moduleId;
        }

        public string Id { get; }
    }

    public class ModuleRegistry
    {
        private static readonly IDictionary<string, Type> moduleTypes = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        private readonly IServiceProvider serviceProvider;

        public ModuleRegistry(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static void RegisterModules(IServiceCollection services)
        {
            services.AddSingleton<ModuleRegistry>();
            FindModuleTypes().ForEach(o => Register(services, o));
        }

        private static IEnumerable<Type> FindModuleTypes()
        {
            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(o => o.GetInterface(nameof(IModule)) == typeof(IModule)
                    && !o.IsAbstract
                    && !o.IsGenericTypeDefinition
                    && !o.IsInterface);
            return moduleTypes;
        }

        private static void Register(IServiceCollection services, Type moduleType)
        {
            var id = GetId(moduleType);
            services.AddSingleton(moduleType);
            moduleTypes[id] = moduleType;

            string GetId(Type moduleType)
            {
                var attribute = moduleType.GetCustomAttribute<ModuleIdAttribute>();
                if (attribute != null)
                {
                    return attribute.Id;
                }

                var name = moduleType.Name;
                if(name.EndsWith("Module"))
                {
                    return name.Remove(name.Length - 6);
                }

                return name;
            }
        }

        public Option<IModule> GetModule(string id)
        {
            var type = moduleTypes.TryGetValue(id);
            var module = type.Map(t => serviceProvider.GetService(t) as IModule);
            return module;
        }
    }
}
