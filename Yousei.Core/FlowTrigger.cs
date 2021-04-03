using Newtonsoft.Json.Linq;
using System;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class FlowTrigger<TArguments> : IFlowTrigger
    {
        public Type ArgumentsType { get; } = typeof(TArguments);

        public IObservable<object> GetEvents(IFlowContext context, object? arguments)
            => GetEvents(context, arguments.SafeCast<TArguments>());

        protected abstract IObservable<object> GetEvents(IFlowContext context, TArguments? arguments);
    }
}