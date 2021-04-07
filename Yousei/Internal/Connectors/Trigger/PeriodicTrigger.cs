using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;

namespace YouseiReloaded.Internal.Connectors.Trigger
{
    internal class PeriodicTrigger : FlowTrigger<PeriodicArguments>
    {
        protected override IObservable<object> GetEvents(IFlowContext context, PeriodicArguments? arguments)
        {
            if (arguments is null)
                throw new ArgumentNullException(nameof(arguments));

            if (arguments.Action is null)
                throw new ArgumentNullException(nameof(arguments));

            return Observable.Create<object>(async (observer, cancellationToken) =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await context.Actor.Act(new[] { arguments.Action }, context);

                        var path = string.IsNullOrEmpty(arguments.Path) ? arguments.Action.Type : arguments.Path;
                        var hasData = await context.ExistsData(path);
                        if (hasData)
                        {
                            var data = await context.GetData(path);
                            observer.OnNext(data ?? Unit.Default);
                        }

                        await Task.Delay(arguments.Interval, cancellationToken);
                    }
                    observer.OnCompleted();
                });
        }
    }
}