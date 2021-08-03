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
    [ExtendObjectType(typeof(Configuration))]
    public class ConfigurationExtension
    {
        public async Task<SourceConfig?> GetConfig(
            [Parent] Configuration configuration,
            [Service] IConfigurationDatabase configurationDatabase)
            => await configurationDatabase.GetConfigurationSource(configuration.Connector, configuration.Name);
    }
}