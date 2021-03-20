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
                Type = "internal.onstart",
            },
            Actions = new BlockConfig[]
            {
                new()
                {
                    Type = "log.write",
                    Arguments = new
                    {
                        Message = "on start.",
                    },
                },
            }
        })).Concat(Observable.Return(("dummy2", new FlowConfig
        {
            Trigger = new()
            {
                Type = "internal.onstop",
            },
            Actions = new BlockConfig[]
            {
                new()
                {
                    Type = "log.write",
                    Arguments = new
                    {
                        Message = "on stop.",
                    }
                }
            }
        })));
    }
}