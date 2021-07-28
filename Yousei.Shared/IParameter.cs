using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IParameter
    {
        Task<object?> Resolve(IFlowContext context);
    }

    public interface IParameter<T> : IParameter
    {
        new Task<T?> Resolve(IFlowContext context);
    }
}