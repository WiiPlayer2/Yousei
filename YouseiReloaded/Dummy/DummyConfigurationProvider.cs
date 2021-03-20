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
                Arguments = new
                {
                    Seconds = 1,
                },
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
                    Arguments = new
                    {
                        Value = new ExpressionParameter("new Random().Next(5)"),
                        Cases = new[]
                            {
                                (0, new BlockConfig[]
                                {
                                    new()
                                    {
                                        Type = "dummy.out",
                                        Arguments = new
                                        {
                                            Text = "was 0.",
                                        },
                                    },
                                }),
                                (1, new BlockConfig[]
                                {
                                    new()
                                    {
                                        Type = "dummy.out",
                                        Arguments = new
                                        {
                                            Text = "was 1.",
                                        },
                                    },
                                }),
                            },
                        Default = new BlockConfig[]
                            {
                                new()
                                {
                                    Type = "internal.sendvalue",
                                    Arguments = new
                                    {
                                        Topic = "DUMMY",
                                        Value = new ExpressionParameter("DateTimeOffset.Now"),
                                    }
                                }
                            },
                    }
                },
            }
        })).Concat(Observable.Return(("dummy2", new FlowConfig
        {
            Trigger = new()
            {
                Type = "internal.onvalue",
                Arguments = new
                {
                    Topic = "DUMMY",
                }
            },
            Actions = new BlockConfig[]
            {
                new()
                {
                    Type = "dummy.out",
                    Arguments = new
                    {
                        Text = new VariableParameter("internal.onvalue"),
                    }
                }
            }
        })));
    }
}