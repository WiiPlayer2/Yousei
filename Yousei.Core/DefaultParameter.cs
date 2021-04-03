using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class DefaultParameter : IParameter
    {
        private DefaultParameter()
        {
        }

        public static DefaultParameter Instance { get; } = new DefaultParameter();

        public Task<T?> Resolve<T>(IFlowContext context)
            => Task.FromResult(default(T));
    }
}