using System;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Connectors.Transmission
{
    internal record GetArguments
    {
        public IParameter<string[]> Fields { get; init; } = Array.Empty<string>().ToConstantParameter();
        public IParameter<int[]> Ids { get; init; } = Array.Empty<int>().ToConstantParameter();
    }
}