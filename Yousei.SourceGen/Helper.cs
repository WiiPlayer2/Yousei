using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Yousei.SourceGen
{
    internal static class Helper
    {
        public static ISymbol? GetSymbol(this SemanticModel semanticModel, SyntaxNode syntaxNode, CancellationToken cancellationToken = default)
        {
            var symbolInfo = semanticModel.GetSymbolInfo(syntaxNode, cancellationToken);
            return symbolInfo.Symbol ?? symbolInfo.CandidateSymbols.FirstOrDefault();
        }

        public static T? GetSymbol<T>(this SemanticModel semanticModel, SyntaxNode syntaxNode, CancellationToken cancellationToken = default)
            where T : class, ISymbol
            => semanticModel.GetSymbol(syntaxNode, cancellationToken) as T;
    }
}