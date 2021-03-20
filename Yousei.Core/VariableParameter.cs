using System;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class VariableParameter : IParameter
    {
        public VariableParameter(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public async Task<T> Resolve<T>(IFlowContext context)
            => (await context.GetData(Path)).Map<T>();
    }
}