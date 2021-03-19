using System;

namespace Yousei.Contracts
{
    public interface IConnector
    {
        Type ConfigurationType { get; }

        IConnection GetConnection(object configuration);
    }
}