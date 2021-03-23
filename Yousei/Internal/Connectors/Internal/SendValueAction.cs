using Yousei.Core;
using System.Reactive.Subjects;
using Yousei.Shared;
using System.Threading.Tasks;

namespace YouseiReloaded.Internal.Connectors.Internal
{
    internal class SendValueAction : FlowAction<SendValueArguments>
    {
        private readonly ISubject<(string, object)> valueSubject;

        public SendValueAction(ISubject<(string, object)> valueSubject)
        {
            this.valueSubject = valueSubject;
        }

        protected override async Task Act(IFlowContext context, SendValueArguments arguments)
        {
            var topic = await arguments.Topic.Resolve<string>(context);
            var value = await arguments.Value.Resolve<object>(context);
            valueSubject.OnNext((topic, value));
        }
    }
}