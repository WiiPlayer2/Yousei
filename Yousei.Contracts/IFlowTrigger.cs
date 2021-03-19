using Newtonsoft.Json.Linq;
using System;

namespace Yousei.Shared
{
    public interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        string Type { get; }

        IObservable<JObject> GetEvents(object arguments);
    }
}