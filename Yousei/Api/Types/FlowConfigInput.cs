using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Api.Types
{
    public record BlockConfigInput(string Type, string Configuration = "default", JToken arguments = default)
    {
        public BlockConfig Map()
            => new()
            {
                Type = Type,
                Configuration = Configuration,
                Arguments = arguments,
            };
    }

    public record FlowConfigInput(BlockConfigInput Trigger, BlockConfigInput[] Actions = default)
    {
        public FlowConfig Map()
            => new()
            {
                Trigger = Trigger?.Map(),
                Actions = (Actions ?? Array.Empty<BlockConfigInput>())
                    .Select(o => o.Map())
                    .ToList(),
            };
    }
}