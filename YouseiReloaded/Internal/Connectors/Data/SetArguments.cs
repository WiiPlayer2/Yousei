using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Data
{
    internal record SetArguments
    {
        public IParameter Path { get; init; }

        public IParameter Value { get; init; }
    }
}