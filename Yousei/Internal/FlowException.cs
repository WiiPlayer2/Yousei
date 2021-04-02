using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    internal class FlowException : Exception
    {
        public FlowException(string message, IFlowContext context, Exception innerException)
            : base(message, innerException)
        {
            Flow = context.Flow;
            Context = context.AsObject().GetAwaiter().GetResult();
        }

        public object Context { get; }

        public string Flow { get; }
    }
}