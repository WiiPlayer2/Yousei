using Newtonsoft.Json.Linq;
using System;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class FlowTrigger<TConnection, TArguments> : IFlowTrigger
        where TConnection : IConnection
    {
        public Type ArgumentsType { get; } = typeof(TArguments);

        public abstract string Name { get; }

        public IObservable<object> GetEvents(IFlowContext context, IConnection connection, object? arguments)
            => GetEvents(context, (TConnection)connection, arguments.SafeCast<TArguments>());

        protected abstract IObservable<object> GetEvents(IFlowContext context, TConnection connection, TArguments? arguments);
    }
}