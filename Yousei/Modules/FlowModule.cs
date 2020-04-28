using LanguageExt;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpYaml.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace Yousei.Modules
{
    public class FlowModule : IModule
    {
        private readonly JobFlowCreator jobFlowCreator;
        private readonly ILogger<JobFlowCreator> logger;
        private readonly string flowsPath;

        private class FlowArguments
        {
            public string FlowFile { get; set; } = string.Empty;

            public Flow Flow { get; set; }

            public JToken Arguments { get; set; }
        }

        private class Flow
        {
            public List<JobAction> Actions { get; set; }
        }

        public FlowModule(JobFlowCreator jobFlowCreator, IConfiguration configuration, ILogger<JobFlowCreator> logger)
        {
            this.jobFlowCreator = jobFlowCreator;
            this.logger = logger;
            flowsPath = configuration.GetValue<string>("Flows");
        }

        public Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<FlowArguments>();
            return GetFlow(args).Match(
                flow =>
                {
                    var actions = flow.Actions.Select(o => new JobAction
                    {
                        ModuleID = o.ModuleID,
                        Arguments = o.Arguments.Map(args.Arguments),
                    }).ToList();
                    var observable = jobFlowCreator.CreateJobFlow(actions, data);
                    return observable;
                },
                () => Observable.Empty<JToken>()).AsTask();
        }

        private Option<Flow> GetFlow(FlowArguments args)
        {
            var directoryInfo = new DirectoryInfo(flowsPath);
            if (!directoryInfo.Exists)
                return None;

            if (args.Flow != null)
                return args.Flow;

            var flow = args.FlowFile;
            var flowFileInfo = new FileInfo(Path.Combine(directoryInfo.FullName, $"{flow}.yaml"));
            var flowFile = flowFileInfo.Exists ? Some(flowFileInfo) : None;
            return flowFile.Bind(
                fileInfo =>
                {
                    try
                    {
                        using var fileStream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        var serializer = new Serializer(new SerializerSettings
                        {
                            ObjectSerializerBackend = JobRegistry.SerializerBackend.Instance,
                        });
                        return Some(serializer.Deserialize<Flow>(fileStream));
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, $"Failed to load flow {flow}");
                        return None;
                    }
                });
        }
    }
}
