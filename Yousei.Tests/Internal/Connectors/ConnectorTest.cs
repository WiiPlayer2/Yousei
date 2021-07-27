using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using YouseiRelaoded.Internal.Connectors.Log;

namespace Yousei.Test.Internal.Connectors
{
    public abstract class ConnectorTest
    {
        protected readonly Mock<IFlowActor> flowActorMock = new();

        protected readonly Mock<IFlowContext> flowContextMock = new();

        [TestInitialize]
        public void Initialize()
        {
            flowContextMock.Setup(o => o.Actor).Returns(flowActorMock.Object);
            flowContextMock.Setup(o => o.ExecutionStack).Returns(new Stack<string>());
        }

        protected Task Act(string name, object? arguments = default, object? configuration = default)
        {
            var connector = CreateConnector();
            var connection = CreateConnection(configuration);
            if (connection is null)
                return Task.CompletedTask;

            var action = connector.GetAction(name);
            if (action is null)
                return Task.CompletedTask;

            return action.Act(flowContextMock.Object, connection, arguments);
        }

        protected IConnection? CreateConnection(object? configuration = default)
            => CreateConnector().GetConnection(configuration);

        protected abstract IConnector CreateConnector();

        protected IObservable<object> Trigger(string name, object? arguments = default, object? configuration = default)
        {
            var connector = CreateConnector();
            var connection = CreateConnection(configuration);
            if (connection is null)
                return Observable.Empty<object>();

            var trigger = connector.GetTrigger(name);
            if (trigger is null)
                return Observable.Empty<object>();

            return trigger.GetEvents(flowContextMock.Object, connection, arguments);
        }
    }
}