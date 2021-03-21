using System;

namespace Yousei.Connectors.Rss
{
    public record Config
    {
        public Uri Url { get; init; }
    }
}