using FluentAssertions;
using FluentAssertions.Reactive;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using YouseiReloaded.Internal.Connectors.Trigger;

namespace Yousei.Test.Internal.Connectors
{
    [TestClass]
    public class TriggerConnectorTest : ConnectorTest
    {
        [TestMethod]
        public void DistinctTriggerRaisesWhenSourceValueChanges()
        {
            // Arrange
            var @base = new BlockConfig
            {
                Type = "testing.test",
            };
            var baseSequence = new object[] { 1, 1, 2, 2, 3, 3, 4, 1 };
            var arguments = new DistinctArguments
            {
                Base = @base,
            };
            flowActorMock.Setup(o => o.GetTrigger(@base, flowContextMock.Object)).Returns(baseSequence.ToObservable());

            // Act
            var trigger = Trigger("distinct", arguments).Observe();

            // Assert
            trigger.Should().Push(5);
            trigger.RecordedMessages.Should().BeEquivalentTo(1, 2, 3, 4, 1);
        }

        [TestMethod]
        public void PeriodicTriggerRaisesPeriodicallyFromAction()
        {
            // Arrange
            var action = new BlockConfig
            {
                Type = "testing.test",
            };
            var arguments = new PeriodicArguments
            {
                Path = "testing.test",
                Interval = TimeSpan.FromMilliseconds(10),
                Action = action,
            };
            flowContextMock.Setup(o => o.ExistsData("testing.test").Result).Returns(true);

            // Act
            var trigger = Trigger("periodic", arguments).Observe();

            // Assert
            trigger.Should().Push(10, TimeSpan.FromSeconds(0.11));
        }

        [TestMethod]
        public void WhenAnyTriggerRaisesWhenOfBaseTriggerRaisesAValue()
        {
            // Arrange
            var triggerBlock1 = new BlockConfig
            {
                Type = "testing.test1",
            };
            var triggerBlock2 = new BlockConfig
            {
                Type = "testing.test2",
            };
            var triggerBlock3 = new BlockConfig
            {
                Type = "testing.test3",
            };
            var subject1 = new Subject<object>();
            var subject2 = new Subject<object>();
            var subject3 = new Subject<object>();
            var arguments = new WhenAnyArguments
            {
                Triggers = new()
                {
                    triggerBlock1,
                    triggerBlock2,
                    triggerBlock3,
                }
            };
            flowActorMock.Setup(o => o.GetTrigger(triggerBlock1, flowContextMock.Object)).Returns(subject1);
            flowActorMock.Setup(o => o.GetTrigger(triggerBlock2, flowContextMock.Object)).Returns(subject2);
            flowActorMock.Setup(o => o.GetTrigger(triggerBlock3, flowContextMock.Object)).Returns(subject3);

            // Act
            var trigger = Trigger("whenany", arguments).Observe();
            subject1.OnNext(1);
            subject2.OnNext(2);
            subject3.OnNext(3);
            subject1.OnNext(1);
            subject2.OnNext(2);
            subject3.OnNext(3);

            // Assert
            trigger.Should().Push(6);
            trigger.RecordedMessages.Should().BeEquivalentTo(
                new { Source = "testing.test1", Data = 1 },
                new { Source = "testing.test2", Data = 2 },
                new { Source = "testing.test3", Data = 3 },
                new { Source = "testing.test1", Data = 1 },
                new { Source = "testing.test2", Data = 2 },
                new { Source = "testing.test3", Data = 3 });
        }

        protected override IConnector CreateConnector()
            => new TriggerConnector();
    }
}