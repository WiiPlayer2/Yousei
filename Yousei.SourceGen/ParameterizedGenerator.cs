using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yousei.SourceGen
{
    [Generator]
    public class ParameterizedGenerator : ISourceGenerator, ISyntaxReceiver
    {
        private readonly List<AttributeSyntax> attributeNodeCandidates = new List<AttributeSyntax>();

        public void Execute(GeneratorExecutionContext context)
        {
            var attributeSource = GetAttributeSource();
            context.AddSource("Attribute.g.cs", attributeSource);

            if (attributeNodeCandidates.Any())
            {
                var compilation = AddAttributeCompilation(attributeSource, context.Compilation);
                var attributeSymbol = compilation.GetTypeByMetadataName("Yousei.SourceGen.ParameterizedAttribute");
                if (attributeSymbol is null)
                    throw new ArgumentNullException(nameof(attributeSymbol));

                foreach (var attributeNode in attributeNodeCandidates)
                {
                    HandleNode(context, attributeNode, compilation, attributeSymbol);
                }
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            attributeNodeCandidates.Clear();
            context.RegisterForSyntaxNotifications(() => this);
        }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is AttributeSyntax attributeSyntax && attributeSyntax.Name.ToString().Contains("Parameterized"))
                attributeNodeCandidates.Add(attributeSyntax);
        }

        private Compilation AddAttributeCompilation(SourceText attributeSource, Compilation compilation)
        {
            var options = (compilation as CSharpCompilation)?.SyntaxTrees[0].Options as CSharpParseOptions;
            return compilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(attributeSource, options));
        }

        private SourceText GetAttributeSource()
        {
            using var stream = typeof(ParameterizedGenerator).Assembly.GetManifestResourceStream("Yousei.SourceGen.ParameterizedAttribute.cs");
            return SourceText.From(stream, Encoding.UTF8, canBeEmbedded: true);
        }

        private void HandleNode(GeneratorExecutionContext context, AttributeSyntax attributeNode, Compilation compilation, INamedTypeSymbol attributeSymbol)
        {
            var semanticModel = compilation.GetSemanticModel(attributeNode.SyntaxTree);
            var currentAttributeSymbol = semanticModel.GetSymbol<IMethodSymbol>(attributeNode, context.CancellationToken)?.ContainingType;

            if (!SymbolEqualityComparer.Default.Equals(attributeSymbol, currentAttributeSymbol))
                return;

            var typeNode = attributeNode.Parent?.Parent as TypeDeclarationSyntax;
            if (typeNode is not RecordDeclarationSyntax
                && typeNode is not ClassDeclarationSyntax
                && typeNode is not StructDeclarationSyntax)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    "YOUSEI01",
                    "Generation",
                    "Attribute must be applied to class, record or struct type.",
                    DiagnosticSeverity.Error,
                    DiagnosticSeverity.Error,
                    true,
                    0,
                    false,
                    location: attributeNode.GetLocation()));
                return;
            }

            if (!typeNode.Modifiers.Any(SyntaxKind.PartialKeyword))
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    "YOUSEI02",
                    "Generation",
                    "Type must be partial.",
                    DiagnosticSeverity.Error,
                    DiagnosticSeverity.Error,
                    true,
                    0,
                    false,
                    location: typeNode.GetLocation()));
                return;
            }

            var type = semanticModel.GetDeclaredSymbol(typeNode);
            if (type is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    "YOUSEI04",
                    "Generation",
                    "Type not resolvable.",
                    DiagnosticSeverity.Error,
                    DiagnosticSeverity.Error,
                    true,
                    0,
                    false,
                    location: typeNode.GetLocation()));
                return;
            }

            var attributeData = type
                .GetAttributes()
                .First(o => SymbolEqualityComparer.Default.Equals(o.AttributeClass, attributeSymbol));
            if (attributeData.ConstructorArguments.Length != 1)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    "YOUSEI03",
                    "Generation",
                    "Wrong argument count.",
                    DiagnosticSeverity.Error,
                    DiagnosticSeverity.Error,
                    true,
                    0,
                    false,
                    location: attributeNode.GetLocation()));
                return;
            }

            var targetType = attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
            if (targetType is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    "YOUSEI04",
                    "Generation",
                    "No target type given.",
                    DiagnosticSeverity.Error,
                    DiagnosticSeverity.Error,
                    true,
                    0,
                    false,
                    location: attributeNode.GetLocation()));
                return;
            }

            var properties = targetType.GetMembers()
                .OfType<IPropertySymbol>();
            var typeKind = typeNode switch
            {
                ClassDeclarationSyntax => "class",
                RecordDeclarationSyntax => "record",
                StructDeclarationSyntax => "struct",
                _ => throw new NotSupportedException(),
            };

            var sb = new StringBuilder();
            sb.AppendLine($"namespace {type.ContainingNamespace} {{");
            sb.AppendLine($"    partial {typeKind} {type.Name} {{");
            foreach (var prop in properties)
            {
                sb.AppendLine($"        public Yousei.Shared.IParameter {prop.Name} {{ get; init; }}");
            }
            sb.AppendLine($"        public async System.Threading.Tasks.Task<global::{targetType}> Resolve(Yousei.Shared.IFlowContext context)");
            sb.AppendLine($"            => new() {{");
            foreach (var prop in properties)
            {
                sb.AppendLine($"                {prop.Name} = await Resolve<{prop.Type}>({prop.Name}, context),");
            }
            sb.AppendLine($"            }};");
            sb.AppendLine($"        private async System.Threading.Tasks.Task<T> Resolve<T>(Yousei.Shared.IParameter parameter, Yousei.Shared.IFlowContext context) {{");
            sb.AppendLine($"            if(parameter is null)");
            sb.AppendLine($"                return default;");
            sb.AppendLine($"            return await parameter.Resolve<T>(context);");
            sb.AppendLine($"        }}");
            sb.AppendLine($"    }}");
            sb.AppendLine($"}}");
            var source = SourceText.From(sb.ToString(), Encoding.UTF8);
            context.AddSource($"{type}.g.cs", source);
        }
    }
}