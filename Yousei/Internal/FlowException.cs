using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace Yousei.Internal
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "RCS1194:Implement exception constructors.",
        Justification = "This is a special exception that's always based on another exception (for now).")]
    internal class FlowException : Exception
    {
        public FlowException(string message, IFlowContext context)
            : base(message)
        {
            Flow = context.Flow;
            Context = context.AsObject().GetAwaiter().GetResult();
            FlowStackTrace = context.GetStackTrace();
        }

        public FlowException(string message, IFlowContext context, Exception innerException)
            : base(message, innerException)
        {
            Flow = context.Flow;
            Context = context.AsObject().GetAwaiter().GetResult();
            FlowStackTrace = context.GetStackTrace();
        }

        public object Context { get; }

        public string Flow { get; }

        public string FlowStackTrace { get; }

        public override string ToString()
            => $"{GetType().FullName}: {Message}{Environment.NewLine}{FlowStackTrace}{Environment.NewLine}{StackTrace}";
    }
}