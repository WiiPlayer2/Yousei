using System;

namespace Yousei.Shared
{
    public interface IFlowTrigger
    {
        Type ArgumentsType { get; }

        string Name { get; }

        IObservable<object> GetEvents(IFlowContext context, IConnection connection, object? arguments);
    }
}