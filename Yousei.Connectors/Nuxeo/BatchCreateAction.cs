using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using NuxeoClient.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Task = System.Threading.Tasks.Task;

namespace Yousei.Connectors.Nuxeo
{
    internal class BatchCreateAction : NuxeoAction<Unit>
    {
        public override string Name { get; } = "batch_create";

        protected override async Task Act(IFlowContext context, Unit _, Client client)
        {
            var batch = await client.Batch();
            await context.SetData(batch);
        }
    }
}