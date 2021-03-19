using System;

namespace Yousei.Shared
{
    public interface IConnector
    {
        Type ConfigurationType { get; }

        IConnection GetConnection(object configuration);
    }
}