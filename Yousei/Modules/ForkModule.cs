using LanguageExt;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Yousei.Modules.Templates;

namespace Yousei.Modules
{
    public class ForkModule : SingleTemplate
    {
        private class Arguments
        {
            public JToken Data { get; set; } = "$";

            public List<JobAction> Actions { get; set; }
        }

        private readonly JobFlowCreator jobFlowCreator;

        public ForkModule(JobFlowCreator jobFlowCreator)
        {
            this.jobFlowCreator = jobFlowCreator;
        }

        public override Task<JToken> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var mappedData = args.Data.Map(data);
            var flowObservable = jobFlowCreator.CreateJobFlow(args.Actions, mappedData);
            flowObservable.Wait();
            return data.AsTask();
        }
    }
}
