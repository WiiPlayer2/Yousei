using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal record SendValueArguments
    {
        public IParameter Topic { get; init; }
        public IParameter Value { get; init; }
    }
}