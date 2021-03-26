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
        private readonly IApi api;

        public FlowExtension(IApi api)
        {
            this.api = api;
        }

        public Task<FlowConfig> GetConfig(
            [Parent] Flow flow)
            => api.ConfigurationDatabase.GetFlow(flow.Name);
    }
}