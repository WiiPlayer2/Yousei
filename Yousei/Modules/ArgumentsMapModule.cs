using Microsoft.Extensions.Logging;
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
        private readonly ILogger<ArgumentsMapModule> logger;
        private readonly ModuleRegistry moduleRegistry;

        public ArgumentsMapModule(ILogger<ArgumentsMapModule> logger, ModuleRegistry moduleRegistry)
        {
            this.logger = logger;
            this.moduleRegistry = moduleRegistry;
        }

        public Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var jobAction = arguments.ToObject<JobAction>();
            var mappedArgs = jobAction.Arguments.Map(data);
            logger.LogDebug($"Run module {jobAction.ModuleID} with mapped arguments {mappedArgs}");
            return moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
                module => module.ProcessAsync(mappedArgs, data, cancellationToken),
                () => Observable.Throw<JToken>(new Exception($"Module {jobAction.ModuleID} not found.")));
        }
    }
}
