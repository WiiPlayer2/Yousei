using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public class ConstantParameter : IParameter
    {
        private readonly object value;

        public ConstantParameter(object value)
        {
            this.value = value;
        }

        public Task<object> Resolve(IFlowContext context)
            => Task.FromResult(value);
    }
}