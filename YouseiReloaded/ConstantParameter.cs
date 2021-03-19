using System.Threading.Tasks;

namespace YouseiReloaded
{
    class ConstantParameter : IParameter
    {
        private readonly object value;

        public ConstantParameter(object value)
        {
            this.value = value;
        }

        public Task<object> Resolve(FlowContext context) => Task.FromResult(value);
    }
}
