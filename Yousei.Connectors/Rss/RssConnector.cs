using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Rss
{
    public class RssConnector : SimpleConnector<Config>
    {
        public RssConnector() : base("rss")
        {
        }

        protected override IConnection CreateConnection(Config configuration)
        {
            throw new NotImplementedException();
        }
    }
}