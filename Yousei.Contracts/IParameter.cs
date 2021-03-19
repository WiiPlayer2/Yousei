using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public interface IParameter
    {
        Task<T> Resolve<T>(IFlowContext context);
    }
}