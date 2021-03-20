using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class ConstantParameter : IParameter
    {
        public ConstantParameter(object value)
        {
            Value = value;
        }

        public object Value { get; }

        public Task<T> Resolve<T>(IFlowContext context)
            => Task.FromResult(Value.Map<T>());
    }
}