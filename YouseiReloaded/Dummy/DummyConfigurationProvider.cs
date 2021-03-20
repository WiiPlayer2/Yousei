using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Yousei.Core;
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

        public IObservable<(string Name, FlowConfig Config)> GetFlows() => Observable.Return(("dummy", new FlowConfig
        {
            Trigger = new()
            {
                Type = "dummy.trigger",
                Arguments = new Dictionary<string, object>
                {
                    { "seconds", 1 },
                }
            },
            Actions = new BlockConfig[]
            {
                new()
                {
                    Type = "dummy.out",
                    Arguments = new Dictionary<string, object>
                    {
                        { "text", new VariableParameter("dummy.trigger") },
                    },
                }
            }
        }));
    }
}