using System;

namespace Yousei.Shared
{
    public interface IConnector
    {
        Type ConfigurationType { get; }

        string Name { get; }

        IConnection GetConnection(object configuration);
    }
}