namespace YouseiReloaded
{
    interface IConnectorRegistry
    {
        void Register(IConnector connector);

        void Unregister(IConnector connector);

        IConnector Get(string name);
    }
}