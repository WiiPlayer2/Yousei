using System;
using Yousei.Shared;

namespace YouseiReloaded.Dummy
{
    internal class DummyConfigurationProvider : IConfigurationProvider
    {
        public object GetConnectionConfiguration(string type, string name) => default;

        public FlowConfig GetFlow(string name)
        {
            throw new NotImplementedException();
        }

        public IObservable<(string Name, FlowConfig Config)> GetFlows()
        {
            throw new NotImplementedException();
        }
    }
}