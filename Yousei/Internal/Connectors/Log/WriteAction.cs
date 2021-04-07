using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class WriteAction : FlowAction<WriteArguments>
    {
        private readonly ILogger logger;

        public WriteAction(ILogger logger)
        {
            this.logger = logger;
        }

        protected override async Task Act(IFlowContext context, WriteArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var level = await arguments.Level.Resolve<LogLevel>(context);
            var message = await arguments.Message.Resolve<object>(context);
            var tag = await arguments.Tag.Resolve<string>(context);

            logger.Log(level, $"[{tag}] {message}");
        }
    }
}