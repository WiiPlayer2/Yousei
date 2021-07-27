using System;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal class RemoveAction : FlowAction<ObjectConnection<Client>, RemoveArguments>
    {
        public override string Name { get; } = "remove";

        protected override async Task Act(IFlowContext context, ObjectConnection<Client> connection, RemoveArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var ids = await arguments.Ids.Resolve<int[]>(context);
            var deleteData = await arguments.DeleteData.Resolve<bool>(context);

            connection.Object.TorrentRemoveAsync(ids, deleteData);
        }
    }
}