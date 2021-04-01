using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Web
{
    public class MainService : IHostedService
    {
        private readonly ILogger<MainService> logger;

        public MainService(ILogger<MainService> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            AppDomain.CurrentDomain.FirstChanceException += (_, e) => logger.LogDebug($"First chance: {e.Exception}");
            AppDomain.CurrentDomain.UnhandledException += (_, e) => logger.LogCritical($"Unhandled{(e.IsTerminating ? " (terminating)" : string.Empty)}: {e.ExceptionObject}");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}