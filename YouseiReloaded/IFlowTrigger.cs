using Newtonsoft.Json.Linq;
using System;

namespace YouseiReloaded
{
    internal interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        string Type { get; }

        IObservable<JObject> GetEvents(object arguments);
    }
}