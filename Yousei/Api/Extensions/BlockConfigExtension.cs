using HotChocolate;
using HotChocolate.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Api.Extensions
{
    [ExtendObjectType(nameof(BlockConfig))]
    public class BlockConfigExtension
    {
        public JToken? GetArguments(
            [Parent] BlockConfig blockConfig)
            => blockConfig.Arguments.Map<JToken>();
    }
}