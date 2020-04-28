using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Transmission.API.RPC;
using Transmission.API.RPC.Entity;
using Yousei.Modules.Templates;

namespace Yousei.Modules
{
    public class TransmissionModule : SingleTemplate
    {
        private class AddTorrentData
        {
            public string Url { get; set; }

            public string DownloadDirectory { get; set; }
        }

        private class GetTorrentData
        {
            public string[] Fields { get; set; } = new string[0];

            public int[] IDs { get; set; } = new int[0];
        }

        private class RemoveTorrentData
        {
            public int[] IDs { get; set; } = new int[0];

            public bool DeleteData { get; set; } = false;
        }

        private enum Action
        {
            AddTorrent,
            GetTorrent,
            RemoveTorrent,
        }

        private class Arguments
        {
            public string Endpoint { get; set; }

            public string Login { get; set; }

            public string Password { get; set; }

            public Action Action { get; set; }
        }

        public override Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var client = new Client(args.Endpoint, login: args.Login, password: args.Password);

            return args.Action switch
            {
                Action.AddTorrent => AddTorrent(client, data, cancellationToken),
                Action.GetTorrent => GetTorrent(client, data, cancellationToken),
                Action.RemoveTorrent => RemoveTorrent(client, data, cancellationToken),
                _ => throw new ArgumentOutOfRangeException(nameof(args.Action)),
            };
        }

        private async Task<JToken> AddTorrent(Client client, JToken data, CancellationToken cancellationToken)
        {
            var request = data.ToObject<AddTorrentData>();

            var newTorrentInfo = await client.TorrentAddAsync(new NewTorrent
            {
                Filename = request.Url,
                DownloadDirectory = request.DownloadDirectory,
            }).ConfigureAwait(false);

            return JToken.FromObject(newTorrentInfo);
        }

        private async Task<JToken> GetTorrent(Client client, JToken data, CancellationToken cancellationToken)
        {
            var args = data.ToObject<GetTorrentData>();

            var torrentsInfo = await client.TorrentGetAsync(args.Fields, args.IDs);
            return JToken.FromObject(torrentsInfo);
        }

        private Task<JToken> RemoveTorrent(Client client, JToken data, CancellationToken cancellationToken)
        {
            var args = data.ToObject<RemoveTorrentData>();

            client.TorrentRemoveAsync(args.IDs, args.DeleteData);
            return JValue.CreateNull().AsTask<JToken>();
        }
    }
}
