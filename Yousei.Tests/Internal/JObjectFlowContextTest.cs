using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;
using Yousei.Internal;

namespace Yousei.Test.Internal
{

    [TestClass]
    public class JObjectFlowContextTest : FlowContextTest
    {
        protected override IFlowContext CreateContext()
            => new JObjectFlowContext(flowActorMock.Object, "testing");
    }
}