using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal record AddArguments
    {
        public IParameter Url { get; init; }
        public IParameter DownloadDirectory { get; init; }
    }
}