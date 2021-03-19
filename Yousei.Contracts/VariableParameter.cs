using System;
using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public class VariableParameter : IParameter
    {
        public VariableParameter(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public async Task<object> Resolve(IFlowContext context)
            => await context.GetData(Path);
    }
}