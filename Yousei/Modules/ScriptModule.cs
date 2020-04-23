using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class Globals
    {
        public Globals(JToken data)
        {
            Data = data;
        }

        public JToken Data { get; }
    }

    internal class ScriptModule : BaseOldModule
    {
        internal enum ScriptType
        {
            CSharp,
        }

        internal class Arguments
        {
            public ScriptType Type { get; set; }

            public string Code { get; set; }
        }

        public string ID => "script";

        public override async Task<IAsyncEnumerable<JToken>> Process(JToken arguments, JToken data, CancellationToken cancellationToken)
        {
            var args = arguments.ToObject<Arguments>();
            var result = args.Type switch
            {
                ScriptType.CSharp => await RunCSharp(args.Code, data, cancellationToken),
                _ => throw new NotSupportedException(),
            };

            if (result is Task resultTask)
            {
                await resultTask.ConfigureAwait(false);
                var taskType = resultTask.GetType();
                if(taskType.IsGenericType)
                {
                    var resultProperty = taskType.GetProperty(nameof(Task<object>.Result));
                    result = resultProperty.GetValue(resultTask);
                }
                else
                {
                    result = null;
                }
            }

            if (result is null)
                return JValue.CreateNull().YieldAsync();

            if (result is IEnumerable resultEnumerable)
                return resultEnumerable.Cast<object>().Select(JToken.FromObject).ToAsyncEnumerable();

            return JToken.FromObject(result).YieldAsync();
        }

        private async Task<object> RunCSharp(string code, JToken data, CancellationToken cancellationToken)
        {
            var script = CSharpScript.Create(
                code,
                globalsType: typeof(Globals),
                options: ScriptOptions.Default
                    .WithImports("System"));

            var state = await script.RunAsync(globals: new Globals(data), cancellationToken: cancellationToken).ConfigureAwait(false);
            return state.ReturnValue;
        }
    }
}
