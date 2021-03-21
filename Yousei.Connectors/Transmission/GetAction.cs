using System.Threading.Tasks;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal class GetAction : FlowAction<GetArguments>
    {
        private readonly Client client;

        public GetAction(Client client)
        {
            this.client = client;
        }

        protected override async Task Act(IFlowContext context, GetArguments arguments)
        {
            var fields = await arguments.Fields.Resolve<string[]>(context);
            var ids = await arguments.Ids.Resolve<int[]>(context);

            await client.TorrentGetAsync(fields, ids);
        }
    }
}