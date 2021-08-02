using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Yousei.Shared;

namespace Yousei.Core
{
    public record ScriptGlobals(dynamic Context);

    public class ExpressionParameter : ExpressionParameter<object?>
    {
        public ExpressionParameter(string expressionCode) : base(expressionCode)
        {
        }
    }

    public class ExpressionParameter<T> : IParameter<T>
    {
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

        public async Task<T?> Resolve(IFlowContext context)
        {
            var contextObj = await context.AsObject();
            var globals = new ScriptGlobals(contextObj);
            var result = await script.RunAsync(
                globals: globals);
            return result.ReturnValue.Map<T>();
        }

        async Task<object?> IParameter.Resolve(IFlowContext context)
            => await Resolve(context);

        public override string ToString()
            => $"=> {Code}";
    }
}