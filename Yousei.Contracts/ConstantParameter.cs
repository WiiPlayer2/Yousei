using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public class ConstantParameter : IParameter
    {
        public ConstantParameter(object value)
        {
            Value = value is null ? null : JToken.FromObject(value);
        }

        public JToken Value { get; }

        public Task<T> Resolve<T>(IFlowContext context)
            => Task.FromResult(Value is null ? default(T) : Value.ToObject<T>());
    }
}