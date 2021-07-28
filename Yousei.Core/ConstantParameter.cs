using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class ConstantParameter : ConstantParameter<object?>
    {
        public ConstantParameter(object? value) : base(value)
        {
        }
    }

    public class ConstantParameter<T> : IParameter<T>
    {
        public ConstantParameter(T value)
        {
            Value = value;
        }

        public T Value { get; }

        public async Task<T?> Resolve(IFlowContext context)
            => await Task.FromResult(Value);

        async Task<object?> IParameter.Resolve(IFlowContext context)
            => await Resolve(context);

        public override string ToString()
            => $"{Value}";
    }
}