using Yousei.Core;
using Yousei.Shared;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;

namespace YouseiRelaoded.Internal.Connectors.Log
{
    internal class WriteAction : FlowAction<UnitConnection, WriteArguments>
    {
        private readonly ILogger logger;

        public WriteAction(ILogger logger)
        {
            this.logger = logger;
        }

        public override string Name { get; } = "write";

        protected override async Task Act(IFlowContext context, UnitConnection _, WriteArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var level = await arguments.Level.Resolve(context);
            var message = await arguments.Message.Resolve(context);
            var tag = await arguments.Tag.Resolve(context);

            logger.Log(level, $"[{tag}] {message}");
        }
    }
}