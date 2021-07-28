using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    internal class MappedParameter : IParameter
    {
        private readonly Type targetType;

        public MappedParameter(IParameter baseParameter, Type targetType)
        {
            Base = baseParameter;
            this.targetType = targetType;
        }

        public IParameter Base { get; }

        public async Task<object?> Resolve(IFlowContext context)
        {
            var value = await Base.Resolve(context);
            return value.Map(targetType);
        }

        public override string ToString()
            => Base.ToString() ?? string.Empty;
    }

    internal class MappedParameter<T> : MappedParameter, IParameter<T>
    {
        public MappedParameter(IParameter baseParameter) : base(baseParameter, typeof(T))
        {
        }

        public new async Task<T?> Resolve(IFlowContext context)
        {
            var mappedValue = await base.Resolve(context);
            return (T?)mappedValue;
        }
    }
}