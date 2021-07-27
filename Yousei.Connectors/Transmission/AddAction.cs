using System;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;
using Yousei.Core;
using Yousei.Shared;
using Yousei.SourceGen;

namespace Yousei.Connectors.Transmission
{
    [Parameterized(typeof(NewTorrent))]
    internal partial record ParameterizedNewTorrent { }

    internal class AddAction : FlowAction<ObjectConnection<Client>, ParameterizedNewTorrent>
    {
        public override string Name { get; } = "add";

        protected override async Task Act(IFlowContext context, ObjectConnection<Client> connection, ParameterizedNewTorrent? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var newTorrent = await arguments.Resolve(context);

            var torrentInfo = await connection.Object.TorrentAddAsync(newTorrent).ConfigureAwait(false);
            await context.SetData(torrentInfo);
        }
    }
}