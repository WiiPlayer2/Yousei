using System;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IFlowAction
    {
        Type ArgumentsType { get; }

        Task Act(IFlowContext context, object? arguments);
    }
}