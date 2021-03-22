using SimpleFeedReader;
using Yousei.Core;

namespace Yousei.Connectors.Rss
{
    internal class RssConnection : SimpleConnection
    {
        public RssConnection(FeedReader feedReader, Config config)
        {
            AddTrigger("feed", new FeedTrigger(feedReader, config));
        }
    }
}