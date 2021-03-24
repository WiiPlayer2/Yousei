using System;
using System.Collections.Generic;

namespace Yousei.Shared
{
    public interface IConfigurationProvider
    {
        object GetConnectionConfiguration(string type, string name);

        IObservable<(string Connector, string Name, object Configuration)> ListConnectionConfigurations();

        FlowConfig GetFlow(string name);

        IObservable<(string Name, FlowConfig Config)> GetFlows();
    }
}