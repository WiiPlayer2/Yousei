using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class ObservableTrigger : IFlowTrigger
    {
        private readonly IObservable<object> observable;

        public ObservableTrigger(IObservable<object> observable)
        {
            this.observable = observable;
        }

        public Type ArgumentsType { get; } = typeof(object);

        public IObservable<object> GetEvents(IFlowContext context, object? arguments)
            => observable;
    }
}