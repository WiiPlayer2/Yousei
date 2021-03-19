using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public interface IParameter
    {
        Task<object> Resolve(IFlowContext context);
    }
}