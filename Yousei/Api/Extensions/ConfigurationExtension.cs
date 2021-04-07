using HotChocolate;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Api.Extensions
{
    [ExtendObjectType(Name = nameof(Configuration))]
    public class ConfigurationExtension
    {
        private readonly IApi api;

        public ConfigurationExtension(IApi api)
        {
            this.api = api;
        }

        public async Task<SourceConfig?> GetConfig(
            [Parent] Configuration configuration)
            => await api.ConfigurationDatabase.GetConfigurationSource(configuration.Connector, configuration.Name);
    }
}