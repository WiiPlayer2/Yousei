using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConnectorRegistry
    {
        IConnector? Get(string name);

        void Register(IConnector connector);

        Task ResetAll();

        void Unregister(IConnector connector);
    }
}