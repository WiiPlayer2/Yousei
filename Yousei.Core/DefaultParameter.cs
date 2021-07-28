using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class DefaultParameter : DefaultParameter<object?>
    {
        public new static DefaultParameter Instance { get; } = new DefaultParameter();
    }

    public class DefaultParameter<T> : IParameter<T>
    {
        protected DefaultParameter()
        {
        }

        public static DefaultParameter<T> Instance { get; } = new DefaultParameter<T>();

        public Task<T?> Resolve(IFlowContext context)
            => Task.FromResult(default(T));

        async Task<object?> IParameter.Resolve(IFlowContext context)
            => await Resolve(context);
    }
}