using System;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConnector
    {
        Type ConfigurationType { get; }

        string Name { get; }

        IFlowAction? GetAction(string name);

        IConnection? GetConnection(object? configuration);

        IFlowTrigger? GetTrigger(string name);

        Task Reset();
    }
}