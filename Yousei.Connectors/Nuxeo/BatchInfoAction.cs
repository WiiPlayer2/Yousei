using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using NuxeoClient.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using Task = System.Threading.Tasks.Task;

namespace Yousei.Connectors.Nuxeo
{
    internal class BatchInfoAction : BatchAction<BatchArguments>
    {
        public BatchInfoAction(NuxeoConfig config) : base(config)
        {
        }

        protected override async Task Act(IFlowContext context, BatchArguments arguments, Batch batch, int? fileIndex)
        {
            if (fileIndex.HasValue)
            {
                var info = await batch.Info(fileIndex.Value);
                await context.SetData(info);
            }
            else
            {
                var info = await batch.Info();
                await context.SetData(info);
            }
        }
    }
}