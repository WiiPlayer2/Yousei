using LanguageExt;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yousei
{
    class ModuleRegistry
    {
        private readonly IDictionary<string, IModule> modules = new Dictionary<string, IModule>();

        public ModuleRegistry() { }

        public Option<IModule> GetModule(string id) => modules.TryGetValue(id);
    }
}
