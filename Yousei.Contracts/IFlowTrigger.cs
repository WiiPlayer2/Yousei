using Newtonsoft.Json.Linq;
using System;

namespace Yousei.Contracts
{
    public interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        string Type { get; }

        IObservable<JObject> GetEvents(object arguments);
    }
}