using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Yousei.Modules
{
    public class ScriptModuleGlobals
    {
        public ScriptModuleGlobals(JToken data, IServiceProvider serviceProvider)
        {
            Data = data;
            JData = data;
            Services = serviceProvider;
        }

        public dynamic Data { get; }

        public JToken JData { get; }

        public IServiceProvider Services { get; }
    }

    public class ScriptModule : IModule
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

        public async Task<IObservable<JToken>> ProcessAsync(JToken arguments, JToken data, CancellationToken cancellationToken)
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
                    return Observable.Return<JToken>(JValue.CreateNull());
                else
                    return Observable.Empty<JToken>();
            }

            if (result is IEnumerable resultEnumerable)
                return resultEnumerable.Cast<object>().Select(JToken.FromObject).ToObservable();

            return Observable.Return(JToken.FromObject(result));
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
                        typeof(JToken).Assembly,
                        typeof(Microsoft.Extensions.Logging.ILogger<>).Assembly)
                    .WithImports(
                        "System",
                        "System.Collections",
                        "System.Collections.Generic",
                        "System.Linq",
                        "Yousei",
                        "Yousei.Modules",
                        "Microsoft.Extensions.Logging",
                        "Newtonsoft.Json.Linq"));

            var state = await script.RunAsync(
                globals: new ScriptModuleGlobals(data, serviceProvider),
                cancellationToken: cancellationToken).ConfigureAwait(false);
            return state.ReturnValue;
        }
    }
}
