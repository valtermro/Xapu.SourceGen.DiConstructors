using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Xapu.SourceGen.DiConstructors.CompilationUtils
{
    public interface ICompilationSymbolParser
    {
        INamedTypeSymbol GetBaseType(INamedTypeSymbol symbol);
        INamespaceSymbol GetNamespace(INamedTypeSymbol symbol);
        IEnumerable<IFieldSymbol> GetOwnFields(INamedTypeSymbol symbol);
        IEnumerable<IMethodSymbol> GetOwnMethods(INamedTypeSymbol symbol);
        IEnumerable<AttributeData> GetAttributes(ISymbol symbol);

        bool IsOverrideMember(ISymbol postControllerMethod);
        string GetNamespaceName(INamedTypeSymbol symbol);
        string GetTypeName(INamedTypeSymbol symbol);
        string GetConstructorName(INamedTypeSymbol symbol);
    }
}
