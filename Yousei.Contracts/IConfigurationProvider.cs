using System;

namespace Yousei.Shared
{
    public interface IConfigurationProvider
    {
        object GetConnectionConfiguration(string type, string name);

        FlowConfig GetFlow(string name);

        IObservable<(string Name, FlowConfig Config)> GetFlows();
    }
}