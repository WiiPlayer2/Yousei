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
    public class ScriptModuleGlobals
    {
        public ScriptModuleGlobals(object data, IServiceProvider serviceProvider)
        {
            Data = data;
            Services = serviceProvider;
        }

        public dynamic Data { get; }

        public IServiceProvider Services { get; }
    }

    public class ScriptModule : BaseOldModule
    {
        private readonly IServiceProvider serviceProvider;

        internal enum ScriptType
        {
            CSharp,
        }

        internal class Arguments
        {
            public ScriptType Type { get; set; }

            public string Code { get; set; }

            public bool EmitNull { get; set; } = false;
        }

        public ScriptModule(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
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
            {
                if (args.EmitNull)
                    return JValue.CreateNull().YieldAsync();
                else
                    return AsyncEnumerable.Empty<JToken>();
            }

            if (result is IEnumerable resultEnumerable)
                return resultEnumerable.Cast<object>().Select(JToken.FromObject).ToAsyncEnumerable();

            return JToken.FromObject(result).YieldAsync();
        }

        private async Task<object> RunCSharp(string code, JToken data, CancellationToken cancellationToken)
        {
            var script = CSharpScript.Create(
                code,
                globalsType: typeof(ScriptModuleGlobals),
                options: ScriptOptions.Default
                    .WithReferences(
                        GetType().Assembly,
                        typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly,
                        typeof(JToken).Assembly)
                    .WithImports(
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Linq",
                        "Yousei",
                        "Yousei.Modules",
                        "Newtonsoft.Json.Linq"));

            var state = await script.RunAsync(
                globals: new ScriptModuleGlobals(data, serviceProvider),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return state.ReturnValue;
        }
    }
}
