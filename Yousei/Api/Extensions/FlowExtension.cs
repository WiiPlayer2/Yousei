using HotChocolate;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Api.Types;
using Yousei.Shared;

namespace Yousei.Api.Extensions
{
    [ExtendObjectType(nameof(Flow))]
    internal class FlowExtension
    {
        public Task<SourceConfig?> GetConfig(
            [Parent] Flow flow,
            [Service] IConfigurationDatabase configurationDatabase)
            => configurationDatabase.GetFlowSource(flow.Name);
    }
}