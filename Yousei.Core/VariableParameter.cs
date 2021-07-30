using System;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class VariableParameter : VariableParameter<object?>
    {
        public VariableParameter(string path) : base(path)
        {
        }
    }

    public class VariableParameter<T> : IParameter<T>
    {
        public VariableParameter(string path)
        {
            Path = path;
        }

        public string Path { get; }

        public async Task<T?> Resolve(IFlowContext context)
            => (await context.GetData(Path)).Map<T>();

        async Task<object?> IParameter.Resolve(IFlowContext context)
            => await Resolve(context);

        public override string ToString()
            => $"-> {Path}";
    }
}