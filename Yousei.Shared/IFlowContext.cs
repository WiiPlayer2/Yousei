using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yousei.Shared
{
    public interface IFlowContext
    {
        IFlowActor Actor { get; }

        Task AddData(string type, object data);

        Task<object> AsObject();

        IFlowContext Clone();

        Task<object> GetData(string path);
    }
}