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
    internal record BatchArguments
    {
        public IParameter ID { get; init; } = DefaultParameter.Instance;

        public IParameter FileIndex { get; init; } = DefaultParameter.Instance;
    }

    internal abstract class BatchAction<T> : NuxeoAction<T>
        where T : BatchArguments
    {
        protected sealed override async Task Act(IFlowContext context, T? arguments, Client client)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var fileIndex = await arguments.FileIndex.Resolve<int?>(context);
            var batch = new Batch
            {
                BatchId = await arguments.ID.Resolve<string>(context),
                FileIndex = fileIndex ?? 0,
            }.SetClient(client);

            await Act(context, arguments, batch, fileIndex);
        }

        protected abstract Task Act(IFlowContext context, T arguments, Batch batch, int? fileIndex);
    }
}