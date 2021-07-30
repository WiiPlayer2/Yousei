using System;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IFlowAction
    {
        Type ArgumentsType { get; }

        string Name { get; }

        Task Act(IFlowContext context, IConnection connection, object? arguments);
    }
}