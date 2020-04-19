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

            var task = Task.Run(() => RunJobAction(job.Actions.First(), JValue.CreateNull(), job.Actions.Skip(1).ToList(), cancellationToken));
            task.ContinueWith(task => logger.LogError(task.Exception, $"Error while running job {job.Name}"), TaskContinuationOptions.OnlyOnFaulted);
            return task;
        }

        private Task RunJobAction(JobAction jobAction, JToken data, IReadOnlyCollection<JobAction> followingJobActions, CancellationToken cancellationToken)
            => moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
                async module =>
                {
                    var results = await module.Process(jobAction.Arguments, data, cancellationToken);
                    var followingTasks = new List<Task>();
                    var nextJobAction = followingJobActions.FirstOrDefault();
                    if (nextJobAction != null)
                    {
                        var nextFollowingJobActions = followingJobActions.Skip(1).ToList();
                        await foreach (var result in results.WithCancellation(cancellationToken))
                        {
                            followingTasks.Add(RunJobAction(nextJobAction, result, nextFollowingJobActions, cancellationToken));
                        }
                        await Task.WhenAll(followingTasks);
                    }
                    else
                    {
                        await foreach (var result in results.WithCancellation(cancellationToken))
                        {
                            if (result.Type != JTokenType.Null)
                            {
                                logger.LogDebug(result.ToString());
                            }
                        }
                    }
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
