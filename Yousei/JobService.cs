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

namespace Yousei
{
    class JobService : IHostedService
    {
        private readonly ILogger logger;
        private readonly JobRegistry jobRegistry;
        private readonly ModuleRegistry moduleRegistry;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IDictionary<Job, (CancellationTokenSource, Task)> runningJobs = new Dictionary<Job, (CancellationTokenSource, Task)>();

        public JobService(ILogger<JobService> logger, JobRegistry jobRegistry, ModuleRegistry moduleRegistry)
        {
            this.logger = logger;
            this.jobRegistry = jobRegistry;
            this.moduleRegistry = moduleRegistry;

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

        private Task RunJob(Job job, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Run job {job.Name}");
            if (!job.Actions.Any())
                return Task.CompletedTask;

            var task = Task.Run(() => RunJobAction(job, job.Actions.First(), JValue.CreateNull(), job.Actions.Skip(1).ToList(), cancellationToken));
            return task.ContinueWith(task =>
            {
                if(task.IsFaulted)
                    logger.LogError(task.Exception, $"Error while running job {job.Name}");
            });
        }

        private Task RunJobAction(Job job, JobAction jobAction, JToken data, IReadOnlyCollection<JobAction> followingJobActions, CancellationToken cancellationToken)
            => moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
                async module =>
                {
                    var tcs = new TaskCompletionSource<bool>();
                    var result = await module.Process(jobAction.Arguments, data, cancellationToken);
                    var nextJobAction = followingJobActions.FirstOrDefault();
                    result.Subscribe(
                        async data =>
                        {
                            if (nextJobAction != null)
                            {
                                try
                                {
                                    await RunJobAction(job, nextJobAction, data, followingJobActions.Skip(1).ToList(), cancellationToken);
                                }
                                catch(Exception e)
                                {
                                    logger.LogError(e, $"Exception while executing module {nextJobAction.ModuleID} in job {job.Name}");
                                }
                            }
                            else
                            {
                                logger.LogDebug($"Result from {job.Name}: {result}");
                            }
                        },
                        exception => logger.LogError(exception, $"Exception while executing module {jobAction.ModuleID} in job {job.Name}"),
                        () => tcs.SetResult(true),
                        cancellationToken);
                    await tcs.Task;
                    return unit;
                },
                () =>
                {
                    logger.LogError($"Module {jobAction.ModuleID} not found.");
                    return unit;
                });

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var runningTasks = runningJobs.ToList();
            cancellationTokenSource.Cancel();
            await Task.WhenAll(runningTasks.Select(o => o.Value.Item2));
        }
    }
}
