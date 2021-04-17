using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using YouseiRelaoded.Internal.Connectors.Log;

namespace Yousei.Test.Internal.Connectors
{
    public abstract class ConnectorTest
    {
        protected readonly Mock<IFlowContext> flowContextMock = new();

        protected Task Act(string name, object? arguments = default, object? configuration = default)
            => CreateConnection(configuration)?.CreateAction(name)?.Act(flowContextMock.Object, arguments) ?? Task.CompletedTask;

        protected IConnection? CreateConnection(object? configuration = default)
            => CreateConnector().GetConnection(configuration);

        protected abstract IConnector CreateConnector();

        protected IObservable<object> Trigger(string name, object? arguments = default, object? configuration = default)
                            => CreateConnection(configuration)?.CreateTrigger(name)?.GetEvents(flowContextMock.Object, arguments) ?? Observable.Empty<object>();
    }
}