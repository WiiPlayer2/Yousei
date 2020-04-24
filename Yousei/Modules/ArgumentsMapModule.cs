using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class ArgumentsMapModule : IModule
    {
        private readonly ModuleRegistry moduleRegistry;

        public ArgumentsMapModule(ModuleRegistry moduleRegistry)
        {
            this.moduleRegistry = moduleRegistry;
        }

        public Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var jobAction = arguments.ToObject<JobAction>();
            var mappedArgs = jobAction.Arguments.Map(data);
            return moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
                module => module.ProcessAsync(mappedArgs, data, cancellationToken),
                () => Observable.Throw<JToken>(new Exception($"Module {jobAction.ModuleID} not found.")));
        }
    }
}
