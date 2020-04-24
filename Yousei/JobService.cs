using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Reactive;
using static LanguageExt.Prelude;
using System.Reactive.Linq;

namespace Yousei
{
    class JobService : IHostedService
    {
        private readonly ILogger logger;
        private readonly JobRegistry jobRegistry;
        private readonly JobFlowCreator jobFlowCreator;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDictionary<Job, (CancellationTokenSource, Task)> runningJobs = new ConcurrentDictionary<Job, (CancellationTokenSource, Task)>();

        public JobService(ILogger<JobService> logger, JobRegistry jobRegistry, JobFlowCreator jobFlowCreator)
        {
            this.logger = logger;
            this.jobRegistry = jobRegistry;
            this.jobFlowCreator = jobFlowCreator;

            jobRegistry.JobAdded += JobRegistry_JobAdded;
            jobRegistry.JobRemoved += JobRegistry_JobRemoved;
        }

        private async void JobRegistry_JobRemoved(object sender, Job job)
        {
            if (runningJobs.TryGetValue(job, out var pair))
            {
                pair.Item1.Cancel();
                await pair.Item2.ConfigureAwait(false);
            }
        }

        private async void JobRegistry_JobAdded(object sender, Job job)
        {
            var cts = new CancellationTokenSource();
            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationTokenSource.Token);
            var task = RunJob(job, linkedCts.Token);
            runningJobs.Add(job, (cts, task));
            await task.ContinueWith(_ => runningJobs.Remove(job));
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            jobRegistry.Initialize();
            return Task.CompletedTask;
        }

        private async Task RunJob(Job job, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Run job {job.Name}");
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            var jobObservable = jobFlowCreator.CreateJobFlow(job.Actions);
            jobObservable.Subscribe(
                data => logger.LogDebug($"Result from {job.Name}: {data}"),
                exception =>
                {
                    logger.LogError(exception, $"Error while running job {job.Name}");
                    tcs.TrySetResult(false);
                },
                () => tcs.TrySetResult(true),
                cancellationToken);
            await tcs.Task.ContinueWith(_ => { });
            logger.LogInformation($"Job {job.Name} finished.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var runningTasks = runningJobs.ToList();
            cancellationTokenSource.Cancel();
            await Task.WhenAll(runningTasks.Select(o => o.Value.Item2));
        }
    }
}
