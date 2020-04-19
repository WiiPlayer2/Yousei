using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;

namespace Yousei.Modules
{
    class TransmissionModule : IModule
    {
        private class Data
        {
            public string Url { get; set; }
        }

        private class Arguments
        {
            public string Url { get; set; }

            public string Login { get; set; }

            public string Password { get; set; }

            public string DownloadDirectory { get; set; }
        }

        public string ID => "transmission";

        public async Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var request = data.ToObject<Data>();

            var client = new Client(args.Url, login: args.Login, password: args.Password);
            var newTorrentInfo = await client.TorrentAddAsync(new NewTorrent
            {
                Filename = request.Url,
                DownloadDirectory = args.DownloadDirectory,
            }).ConfigureAwait(false);

            return JToken.FromObject(newTorrentInfo).YieldAsync();
        }
    }
}
