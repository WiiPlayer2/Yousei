namespace Yousei.Shared
{
    public interface IConnectorRegistry
    {
        IConnector Get(string name);

        void Register(IConnector connector);

        void Unregister(IConnector connector);
    }
}