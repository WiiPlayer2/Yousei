using System;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal class GetAction : FlowAction<ObjectConnection<Client>, GetArguments>
    {
        public override string Name { get; } = "get";

        protected override async Task Act(IFlowContext context, ObjectConnection<Client> connection, GetArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var fields = await arguments.Fields.Resolve(context);
            var ids = await arguments.Ids.Resolve(context);

            var torrents = await connection.Object.TorrentGetAsync(fields, ids);
            await context.SetData(torrents);
        }
    }
}