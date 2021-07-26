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

        public ObservableTrigger(string name, IObservable<object> observable)
        {
            Name = name;
            this.observable = observable;
        }

        public Type ArgumentsType { get; } = typeof(object);

        public string Name { get; }

        public IObservable<object> GetEvents(IFlowContext context, IConnection connection, object? arguments)
            => observable;
    }
}