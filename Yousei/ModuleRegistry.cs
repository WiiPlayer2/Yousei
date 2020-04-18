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

        public ModuleRegistry()
        {
            Register(new ShellModule());
        }

        public void Register(IModule module) => modules[module.ID] = module;

        public Option<IModule> GetModule(string id) => modules.TryGetValue(id);
    }
}
