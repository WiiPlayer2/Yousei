using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IParameter
    {
        Task<T?> Resolve<T>(IFlowContext context);
    }
}