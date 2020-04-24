using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class LogModule : IModule
    {
        private readonly ILogger<LogModule> logger;

        private class Arguments
        {
            public LogLevel Level { get; set; } = LogLevel.Information;

            public string Format { get; set; } = string.Empty;

            public string[] Values { get; set; } = new string[0];
        }

        public LogModule(ILogger<LogModule> logger)
        {
            this.logger = logger;
        }

        public Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var values = args.Values.Select(path => data.Get(path)).Cast<object>().ToArray();
            var message = string.Format(args.Format, values);
            logger.Log(args.Level, message);
            return Task.FromResult(Observable.Return(data));
        }
    }
}
