using System;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal record RemoveArguments
    {
        public IParameter<int[]> Ids { get; init; } = Array.Empty<int>().ToConstantParameter();
        public IParameter<bool> DeleteData { get; init; } = false.ToConstantParameter();
    }
}