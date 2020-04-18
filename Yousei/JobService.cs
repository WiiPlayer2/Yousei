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
        private readonly ISet<Task> runningJobs = new HashSet<Task>();

        public JobService(ILogger logger, JobRegistry jobRegistry, ModuleRegistry moduleRegistry)
        {
            this.logger = logger;
            this.jobRegistry = jobRegistry;
            this.moduleRegistry = moduleRegistry;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            foreach(var job in jobRegistry.Jobs)
            {
                RunJob(job, cancellationTokenSource.Token);
            }

            return Task.CompletedTask;
        }

        private async void RunJob(Job job, CancellationToken cancellationToken)
        {
            logger.LogInformation($"Run job {job.Name}");
            if (!job.Actions.Any())
                return;

            var task = Task.Run(() => RunJobAction(job.Actions.First(), JValue.CreateNull(), job.Actions.Skip(1).ToList(), cancellationToken));
            runningJobs.Add(task);
            await task.ContinueWith(_ => runningJobs.Remove(task));
        }

        private Task RunJobAction(JobAction jobAction, JToken data, IReadOnlyCollection<JobAction> followingJobActions, CancellationToken cancellationToken)
        {
            return moduleRegistry.GetModule(jobAction.ModuleID).MatchAsync(
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
                        await foreach(var result in results.WithCancellation(cancellationToken))
                        {
                            if(result.Type != JTokenType.Null)
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
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            var runningTasks = runningJobs.ToList();
            cancellationTokenSource.Cancel();
            await Task.WhenAll(runningTasks);
        }
    }
}
