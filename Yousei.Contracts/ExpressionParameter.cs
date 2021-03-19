using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public class ExpressionParameter : IParameter
    {
        public ExpressionParameter(string expressionCode)
        {
            // Compile expression into script
            throw new System.NotImplementedException();
        }

        public Task<object> Resolve(IFlowContext context)
        {
            // Resolve by running script
            throw new System.NotImplementedException();
        }
    }
}