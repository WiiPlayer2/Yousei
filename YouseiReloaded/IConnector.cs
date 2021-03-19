using System;

namespace YouseiReloaded
{
    interface IConnector
    {
        Type ConfigurationType { get; }

        IConnection GetConnection(object configuration);
    }
}