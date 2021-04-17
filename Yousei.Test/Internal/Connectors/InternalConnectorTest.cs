using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Core;
using Yousei.Shared;
using YouseiReloaded.Internal.Connectors.Internal;

namespace Yousei.Test.Internal.Connectors
{
    [TestClass]
    public class InternalConnectorTest : ConnectorTest
    {
        private readonly EventHub eventHub = new EventHub();

        [TestMethod]
        public void OnEventTriggerRaisesEventHubEvent()
        {
            // Act
            var trigger = Trigger("onevent").Observe();
            eventHub.RaiseEvent(InternalEvent.FlowUpdated, "asdf");

            // Assert
            using (new AssertionScope())
            {
                trigger.Should().Push(1);
                trigger.RecordedMessages.Should().BeEquivalentTo(new
                {
                    Event = InternalEvent.FlowUpdated,
                    Data = "asdf",
                });
            }
        }

        [TestMethod]
        public void OnExceptionTriggerRaisesEventHubExceptionEvent()
        {
            // Arrange
            var exception = new Exception();

            // Act
            var trigger = Trigger("onexception").Observe();
            eventHub.RaiseEvent(InternalEvent.Exception, exception);

            // Assert
            using (new AssertionScope())
            {
                trigger.Should().Push(1);
                trigger.RecordedMessages.Should().BeEquivalentTo(exception);
            }
        }

        [TestMethod]
        public void OnStartTriggerRaisesEventHubStartEvent()
        {
            // Act
            var trigger = Trigger("onstart").Observe();
            eventHub.RaiseEvent(InternalEvent.Start);

            // Assert
            using (new AssertionScope())
            {
                trigger.Should().Push(1);
                trigger.Should().Complete();
            }
        }

        [TestMethod]
        public void OnStopTriggerRaisesEventHubStopEvent()
        {
            // Act
            var trigger = Trigger("onstop").Observe();
            eventHub.RaiseEvent(InternalEvent.Stop);

            // Assert
            using (new AssertionScope())
            {
                trigger.Should().Push(1);
                trigger.Should().Complete();
            }
        }

        [TestMethod]
        public void OnValueTriggerRaisesEventHubValue()
        {
            // Arrange
            var arguments = new OnValueConfiguration
            {
                Topic = "testing",
            };

            // Act
            var trigger = Trigger("onvalue", arguments).Observe();
            eventHub.Values.OnNext(("testing", 6969));

            // Assert
            using (new AssertionScope())
            {
                trigger.Should().Push(1);
                trigger.RecordedMessages.Should().BeEquivalentTo(6969);
            }
        }

        [TestMethod]
        public async Task ReloadActionRaisesReloadOnEventHub()
        {
            // Arrange
            var values = eventHub.Reload.Observe();

            // Act
            await Act("reload");

            // Assert
            using (new AssertionScope())
            {
                values.Should().Push(1);
            }
        }

        [TestMethod]
        public async Task SendValueActionRaisesValueOnEventHub()
        {
            // Arrange
            var arguments = new SendValueArguments
            {
                Topic = "testing".ToConstantParameter(),
                Value = 42.ToConstantParameter(),
            };
            var values = eventHub.Values.Observe();

            // Act
            await Act("sendvalue", arguments);

            // Assert
            using (new AssertionScope())
            {
                values.Should().Push(1);
                values.RecordedMessages.Should().BeEquivalentTo(("testing", 42));
            }
        }

        protected override IConnector CreateConnector()
            => new InternalConnector(eventHub);
    }
}