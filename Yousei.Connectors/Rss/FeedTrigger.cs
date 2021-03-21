using SimpleFeedReader;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;

namespace Yousei.Connectors.Rss
{
    internal class FeedTrigger : FlowTrigger<Unit>
    {
        private readonly IObservable<FeedItem> observable;

        public FeedTrigger(FeedReader feedReader, Config config)
        {
            observable = Observable.Create<FeedItem>(async (observer, cancellationToken) =>
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

        protected override IObservable<object> GetEvents(Unit arguments)
            => observable;
    }
}