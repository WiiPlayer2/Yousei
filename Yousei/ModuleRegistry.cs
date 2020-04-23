using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
            Register<ShellModule>(services);
            Register<GraphQLModule>(services);
            Register<MapModule>(services);
            Register<InitModule>(services);
            Register<TransmissionModule>(services);
            Register<FilterModule>(services);
            Register<ForeachModule>(services);
            Register<ScriptModule>(services);
            Register<LogModule>(services);
            Register<SinkModule>(services);
            Register<FlowModule>(services);
            Register<ArgumentsMapModule>(services);
        }

        private static void Register<TModule>(IServiceCollection services) where TModule : IModule
        {
            var moduleType = typeof(TModule);
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
