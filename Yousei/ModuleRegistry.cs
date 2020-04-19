using LanguageExt;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Yousei.Modules;

namespace Yousei
{
    class ModuleRegistry
    {
        private readonly IDictionary<string, IModule> modules = new Dictionary<string, IModule>();

        public ModuleRegistry(ILoggerFactory loggerFactory)
        {
            Register(new ShellModule());
            Register(new GraphQLModule(loggerFactory.CreateLogger<GraphQLModule>()));
            Register(new MapModule());
            Register(new InitModule());
            Register(new TransmissionModule());
        }

        public void Register(IModule module) => modules[module.ID] = module;

        public Option<IModule> GetModule(string id) => modules.TryGetValue(id);
    }
}
