using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Xapu.SourceGen.DiConstructors.CompilationUtils
{
    public interface ICompilationSymbolParser
    {
        INamedTypeSymbol GetBaseType(INamedTypeSymbol symbol);
        IEnumerable<IFieldSymbol> GetFields(INamedTypeSymbol symbol);
        INamespaceSymbol GetNamespace(INamedTypeSymbol symbol);

        bool HasInjectedAttribute(IFieldSymbol symbol);
        string GetNamespaceName(INamedTypeSymbol symbol);
        string GetTypeName(INamedTypeSymbol symbol);
        string GetConstructorName(INamedTypeSymbol symbol);
    }
}
