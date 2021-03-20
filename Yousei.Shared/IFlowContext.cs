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

        string CurrentType { get; set; }

        Task<object> AsObject();

        Task ClearData(string path);

        IFlowContext Clone();

        Task<bool> ExistsData(string path);

        Task<object> GetData(string path);

        Task SetData(object data);

        Task SetData(string path, object data);
    }
}