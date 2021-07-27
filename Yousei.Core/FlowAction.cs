using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class FlowAction : FlowAction<Unit>
    {
        protected abstract Task Act(IFlowContext context);

        protected sealed override Task Act(IFlowContext context, Unit arguments)
            => Act(context);
    }

    public abstract class FlowAction<TArguments> : FlowAction<UnitConnection, TArguments>
    {
        protected sealed override Task Act(IFlowContext context, UnitConnection connection, TArguments? arguments)
            => Act(context, arguments);

        protected abstract Task Act(IFlowContext context, TArguments? arguments);
    }

    public abstract class FlowAction<TConnection, TArguments> : IFlowAction
        where TConnection : IConnection
    {
        public Type ArgumentsType { get; } = typeof(TArguments);

        public abstract string Name { get; }

        public Task Act(IFlowContext context, IConnection connection, object? arguments)
            => Act(context, (TConnection)connection, arguments.SafeCast<TArguments>());

        protected abstract Task Act(IFlowContext context, TConnection connection, TArguments? arguments);
    }
}