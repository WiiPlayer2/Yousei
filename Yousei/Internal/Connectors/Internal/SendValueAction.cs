using Yousei.Core;
using System.Reactive.Subjects;
using Yousei.Shared;
using System.Threading.Tasks;
using System;

namespace Yousei.Internal.Connectors.Internal
{
    internal class SendValueAction : FlowAction<UnitConnection, SendValueArguments>
    {
        private readonly ISubject<(string, object?)> valueSubject;

        public SendValueAction(ISubject<(string, object?)> valueSubject)
        {
            this.valueSubject = valueSubject;
        }

        public override string Name { get; } = "sendvalue";

        protected override async Task Act(IFlowContext context, UnitConnection _, SendValueArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            var topic = await arguments.Topic.Resolve<string>(context);
            var value = await arguments.Value.Resolve<object>(context);

            if (topic is null)
                throw new ArgumentNullException(nameof(arguments.Topic));

            valueSubject.OnNext((topic, value));
        }
    }
}