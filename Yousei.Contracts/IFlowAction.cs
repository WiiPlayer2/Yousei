using System;
using System.Threading.Tasks;

namespace Yousei.Contracts
{
    public interface IFlowAction
    {
        Type ArgumentsType { get; }

        string Type { get; }

        Task Act(IFlowContext context, object arguments);
    }
}