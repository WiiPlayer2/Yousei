using System;

namespace Yousei.Shared
{
    public interface IConnection : IDisposable
    {
        IFlowAction? CreateAction(string name);

        IFlowTrigger? CreateTrigger(string name);
    }
}