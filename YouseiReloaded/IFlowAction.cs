using System;
using System.Threading.Tasks;

namespace YouseiReloaded
{
    internal interface IFlowAction
    {
        Type ArgumentsType { get; }

        string Type { get; }

        Task Act(FlowContext context, object arguments);
    }
}