using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConnector
    {
        Type ConfigurationType { get; }

        string Name { get; }

        IFlowAction? GetAction(string name);

        IEnumerable<IFlowAction> GetActions();

        IConnection? GetConnection(object? configuration);

        IFlowTrigger? GetTrigger(string name);

        IEnumerable<IFlowTrigger> GetTriggers();

        Task Reset();
    }
}