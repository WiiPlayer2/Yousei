using System.Threading.Tasks;

namespace YouseiReloaded
{
    internal class ExpressionParameter : IParameter
    {
        public ExpressionParameter(string expressionCode)
        {
            // Compile expression into script
            throw new System.NotImplementedException();
        }

        public Task<object> Resolve(FlowContext context)
        {
            // Resolve by running script
            throw new System.NotImplementedException();
        }
    }
}