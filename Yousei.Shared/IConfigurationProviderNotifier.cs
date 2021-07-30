using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IConfigurationProviderNotifier
    {
        ISubject<(string Name, FlowConfig? Flow)> Flows { get; }
    }
}