using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class GenericFlowContextFactory : IFlowContextFactory
    {
        private readonly Func<IFlowActor, string, IFlowContext> create;

        public GenericFlowContextFactory(Func<IFlowActor, string, IFlowContext> create)
        {
            this.create = create;
        }

        public IFlowContext Create(IFlowActor actor, string flowName)
            => create(actor, flowName);
    }
}