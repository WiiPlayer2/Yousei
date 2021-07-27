using SimpleFeedReader;
using System.Collections.Generic;
using System.Text;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Rss
{
    public class RssConnector : SimpleConnector<Config>
    {
        private readonly FeedReader feedReader = new FeedReader();

        public RssConnector()
        {
            AddTrigger(new FeedTrigger(feedReader));
        }

        public override string Name { get; } = "rss";

        protected override IConnection CreateConnection(Config configuration)
            => ObjectConnection.From(configuration);
    }
}