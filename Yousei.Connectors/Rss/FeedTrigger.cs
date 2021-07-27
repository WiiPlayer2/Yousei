using SimpleFeedReader;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;

namespace Yousei.Connectors.Rss
{
    internal class FeedTrigger : ObservableTrigger<ObjectConnection<Config>>
    {
        public FeedTrigger(FeedReader feedReader)
            : base("feed", c => CreateObservable(feedReader, c.Object))
        {
        }

        private static IObservable<object> CreateObservable(FeedReader feedReader, Config config)
        {
            if (config.Url is null)
                throw new ArgumentNullException(nameof(config.Url));

            return Observable.Create<FeedItem>(async (observer, cancellationToken) =>
                {
                    var lastItem = feedReader.RetrieveFeed(config.Url.ToString())
                        .OrderBy(o => o.PublishDate)
                        .LastOrDefault();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(config.Interval, cancellationToken);
                        var items = feedReader.RetrieveFeed(config.Url.ToString());
                        var checkDate = lastItem?.PublishDate ?? DateTimeOffset.MinValue;
                        foreach (var item in items
                            .OrderBy(o => o.PublishDate)
                            .Where(o => o.PublishDate > checkDate))
                        {
                            observer.OnNext(item);
                            lastItem = item;
                        }
                    }
                    observer.OnCompleted();
                })
                .Publish()
                .RefCount();
        }
    }
}