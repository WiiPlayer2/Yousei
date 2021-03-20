﻿using Newtonsoft.Json.Linq;
using System;
using Yousei.Shared;

namespace Yousei.Core
{
    public abstract class FlowTrigger<TArguments> : IFlowTrigger
    {
        public Type ArgumentsType { get; } = typeof(TArguments);

        public IObservable<object> GetEvents(object arguments)
            => GetEvents(arguments.SafeCast<TArguments>());

        protected abstract IObservable<object> GetEvents(TArguments arguments);
    }
}