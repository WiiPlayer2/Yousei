using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using System.Net;
using System.Reactive.Linq;

namespace Yousei.Connectors.Http
{
    public class HttpConnector : SimpleConnector<Config>
    {
        public HttpConnector()
        {
            DefaultConnection = new HttpConnection(default);

            AddTrigger<WebhookTrigger>();
            AddAction<RequestAction>();
        }

        public override string Name { get; } = "http";

        protected override IConnection? CreateConnection(Config configuration)
        {
            if (!configuration.Prefixes.Any())
            {
                return default;
            }

            var listener = new HttpListener();
            configuration.Prefixes.ForEach(listener.Prefixes.Add);
            return new HttpConnection(listener);
        }
    }
}