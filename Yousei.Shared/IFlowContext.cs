using Newtonsoft.Json.Linq;
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

        Task AddData(string type, JToken data);

        Task<JObject> AsObject();

        IFlowContext Clone();

        Task<JToken> GetData(string path);
    }
}