using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei
{
    public class JobFlowCreator
    {
        private readonly ModuleRegistry moduleRegistry;

        public JobFlowCreator(ModuleRegistry moduleRegistry)
        {
            this.moduleRegistry = moduleRegistry;
        }

        public IObservable<JToken> CreateJobFlow(IEnumerable<JobAction> jobActions)
        {
            var seed = Observable.Return<JToken>(JValue.CreateNull());
            return jobActions.Aggregate(seed, Aggregate);

            IObservable<JToken> Aggregate(IObservable<JToken> acc, JobAction jobAction) => acc.SelectMany((data, token)
                => MergeAsync(jobAction, data, token)).SelectMany(o => o);

            Task<IObservable<JToken>> MergeAsync(JobAction jobAction, JToken data, CancellationToken cancellationToken) =>
                moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
                    module => module.ProcessAsync(jobAction.Arguments, data, cancellationToken),
                    () => Observable.Throw<JToken>(new Exception($"Module {jobAction.ModuleID} not found.")));
        }
    }
}
