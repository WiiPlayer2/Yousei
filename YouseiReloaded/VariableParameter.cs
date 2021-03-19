using System;
using System.Threading.Tasks;

namespace YouseiReloaded
{
    internal class VariableParameter : IParameter
    {
        public VariableParameter(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public async Task<object> Resolve(FlowContext context) => await context.GetData(Path);
    }
}