using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class LoopModule : IModule
    {
        private readonly JobFlowCreator jobFlowCreator;

        private class Arguments
        {
            public string Until { get; set; } = "$";

            public List<JobAction> Actions { get; set; }
        }

        public LoopModule(JobFlowCreator jobFlowCreator)
        {
            this.jobFlowCreator = jobFlowCreator;
        }

        public Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var args = arguments.ToObject<Arguments>();
            var flowObservable = jobFlowCreator.CreateJobFlow(args.Actions, data);
            return flowObservable.SelectMany(async (JToken prevData, CancellationToken token) =>
            {
                token.ThrowIfCancellationRequested();
                if (prevData.Get(args.Until).ToObject<bool>())
                    return Observable.Return(data);
                return await ProcessAsync(arguments, data, token);
            }).SelectMany(o => o).AsTask();
        }
    }
}
