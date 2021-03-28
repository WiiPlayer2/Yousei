using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Web.Model
{
    public abstract class ConfigModel
    {
        public ConfigModel(IConfigurationDatabase database)
        {
            Database = database;
        }

        protected IConfigurationDatabase Database { get; }

        public abstract Task<string> Load();

        public abstract Task Save(string content);
    }
}