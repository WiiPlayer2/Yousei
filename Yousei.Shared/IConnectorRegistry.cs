using System.Collections.Generic;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConnectorRegistry
    {
        IConnector? Get(string name);

        IEnumerable<IConnector> GetAll();

        void Register(IConnector connector);

        Task ResetAll();

        void Unregister(IConnector connector);
    }
}