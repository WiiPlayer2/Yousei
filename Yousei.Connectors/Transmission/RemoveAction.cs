using System;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal class RemoveAction : FlowAction<RemoveArguments>
    {
        private readonly Client client;

        public RemoveAction(Client client)
        {
            this.client = client;
        }

        protected override async Task Act(IFlowContext context, RemoveArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var ids = await arguments.Ids.Resolve<int[]>(context);
            var deleteData = await arguments.DeleteData.Resolve<bool>(context);

            client.TorrentRemoveAsync(ids, deleteData);
        }
    }
}