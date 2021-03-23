using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Trigger
{
    internal class PeriodicTrigger : FlowTrigger<PeriodicArguments>
    {
        protected override IObservable<object> GetEvents(IFlowContext context, PeriodicArguments arguments)
            => Observable.Create<object>(async (observer, cancellationToken) =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    await context.Actor.Act(new[] { arguments.Action }, context);

                    var path = string.IsNullOrEmpty(arguments.Path) ? arguments.Action.Type : arguments.Path;
                    var hasData = await context.ExistsData(path);
                    if (hasData)
                    {
                        var data = await context.GetData(path);
                        observer.OnNext(data);
                    }

                    await Task.Delay(arguments.Interval, cancellationToken);
                }
                observer.OnCompleted();
            });
    }
}