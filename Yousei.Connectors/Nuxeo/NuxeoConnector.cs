using Newtonsoft.Json.Linq;
using NuxeoClient;
using NuxeoClient.Adapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Nuxeo
{
    public record NuxeoConfig
    {
        public string Url { get; init; } = string.Empty;

        public string Username { get; init; } = string.Empty;

        public string Password { get; init; } = string.Empty;
    }

    public class NuxeoConnector : SimpleConnector<NuxeoConfig>
    {
        public NuxeoConnector() : base("nuxeo")
        {
        }

        protected override IConnection? CreateConnection(NuxeoConfig configuration)
            => new NuxeoConnection(configuration);
    }
}