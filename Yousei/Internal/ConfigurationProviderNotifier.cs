using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Internal
{
    public class ConfigurationProviderNotifier : IConfigurationProviderNotifier
    {
        public ISubject<(string Name, FlowConfig Flow)> Flows { get; } = new Subject<(string, FlowConfig)>();
    }
}