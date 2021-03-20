using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public class ExpressionParameter : IParameter
    {
        public record ScriptGlobals(dynamic Context);

        private readonly Script<object> script;

        public ExpressionParameter(string expressionCode)
        {
            Code = expressionCode;

            var scriptOptions = ScriptOptions.Default
                .WithLanguageVersion(LanguageVersion.Latest)
                .AddReferences(typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly)
                .AddImports("System");
            script = CSharpScript.Create(
                expressionCode,
                options: scriptOptions,
                globalsType: typeof(ScriptGlobals));
        }

        public string Code { get; }

        public async Task<T> Resolve<T>(IFlowContext context)
        {
            var contextObj = await context.AsObject();
            var globals = new ScriptGlobals(contextObj);
            var result = await script.RunAsync(
                globals: globals);
            return result.ReturnValue.Map<T>();
        }
    }
}