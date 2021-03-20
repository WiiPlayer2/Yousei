using System;

namespace Yousei.Shared
{
    public interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        IObservable<object> GetEvents(object arguments);
    }
}