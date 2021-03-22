using System.Threading.Tasks;
using Transmission.API.RPC;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal class AddAction : FlowAction<AddArguments>
    {
        private readonly Client client;

        public AddAction(Client client)
        {
            this.client = client;
        }

        protected override async Task Act(IFlowContext context, AddArguments arguments)
        {
            var url = await arguments.Url.Resolve<string>(context);
            var downloadDirectory = await arguments.DownloadDirectory.Resolve<string>(context);

            var torrentInfo = await client.TorrentAddAsync(new()
            {
                Filename = url,
                DownloadDirectory = downloadDirectory
            }).ConfigureAwait(false);
            await context.SetData(torrentInfo);
        }
    }
}