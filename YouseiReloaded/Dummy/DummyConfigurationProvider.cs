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
                //new()
                //{
                //    Type = "data.set",
                //    Arguments = new Dictionary<string, object>
                //    {
                //        { "path", "vars.list" },
                //        { "value", new[]{ 1, 2, 3, 4} },
                //    },
                //},
                new()
                {
                    Type = "control.switch",
                    Arguments = new Dictionary<string, object>
                    {
                        { "value", new ExpressionParameter("new Random().Next(5)") },
                        { "cases", new[]
                            {
                                (0, new BlockConfig[]
                                {
                                    new()
                                    {
                                        Type = "dummy.out",
                                        Arguments = new Dictionary<string, object>
                                        {
                                            { "text", "was 0." },
                                        },
                                    },
                                }),
                                (1, new BlockConfig[]
                                {
                                    new()
                                    {
                                        Type = "dummy.out",
                                        Arguments = new Dictionary<string, object>
                                        {
                                            { "text", "was 1." },
                                        },
                                    },
                                }),
                            }
                        },
                        {"default", new BlockConfig[]
                            {
                                new()
                                {
                                    Type = "internal.sendvalue",
                                    Arguments = new Dictionary<string, object>
                                    {
                                        {"topic", "DUMMY" },
                                        {"value", new ExpressionParameter("DateTimeOffset.Now") },
                                    }
                                }
                            }
                        }
                    }
                },
            }
        })).Concat(Observable.Return(("dummy2", new FlowConfig
        {
            Trigger = new()
            {
                Type = "internal.onvalue",
                Arguments = new Dictionary<string, object>
                {
                    { "topic", "DUMMY" }
                }
            },
            Actions = new BlockConfig[]
            {
                new()
                {
                    Type = "dummy.out",
                    Arguments = new Dictionary<string, object>
                    {
                        { "text", new VariableParameter("internal.onvalue") }
                    }
                }
            }
        })));
    }
}