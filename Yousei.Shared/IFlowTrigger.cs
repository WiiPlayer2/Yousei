using Newtonsoft.Json.Linq;
using System;

namespace Yousei.Shared
{
    public interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        IObservable<JToken> GetEvents(object arguments);
    }
}