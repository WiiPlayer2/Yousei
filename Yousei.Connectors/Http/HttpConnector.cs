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
    public class HttpConnector : Connector<Config>
    {
        private readonly Dictionary<Config, HttpListener> httpListeners = new();

        public HttpConnector() : base("http")
        {
        }

        protected override IConnection GetConnection(Config configuration)
        {
            if (configuration is null)
                return new HttpConnection(default);

            if (!httpListeners.TryGetValue(configuration, out var listener))
            {
                if (configuration.Prefixes.Any())
                {
                    listener = new HttpListener();
                    configuration.Prefixes.ForEach(listener.Prefixes.Add);
                }
            }
            return new HttpConnection(listener);
        }
    }
}