using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Xapu.SourceGen.DiConstructors.CompilationUtils
{
    internal class DefaultCompilationSymbolParser : ICompilationSymbolParser
    {
        public bool IsClass(INamedTypeSymbol symbol)
        {
            return symbol.TypeKind == TypeKind.Class;
        }

        public bool IsPartial(INamedTypeSymbol symbol)
        {
            foreach (var syntaxReference in symbol.DeclaringSyntaxReferences)
            {
                var syntax = syntaxReference.GetSyntax();

                if (!(syntax is TypeDeclarationSyntax typeDeclaration))
                    continue;

                foreach (var modifier in typeDeclaration.Modifiers)
                {
                    if (modifier.ValueText == "partial")
                        return true;
                }
            }

            return false;
        }

        public INamedTypeSymbol GetBaseType(INamedTypeSymbol symbol)
        {
            if (symbol.BaseType is INamedTypeSymbol baseType)
                return baseType;
            else
                return null;
        }

        public IEnumerable<IFieldSymbol> GetOwnFields(INamedTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member is IFieldSymbol field)
                    yield return field;
            }
        }

        public IEnumerable<IMethodSymbol> GetOwnMethods(INamedTypeSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                if (member is IMethodSymbol method)
                    yield return method;
            }
        }

        public IEnumerable<AttributeData> GetAttributes(ISymbol symbol)
        {
            return symbol.GetAttributes();
        }

        public INamespaceSymbol GetNamespace(INamedTypeSymbol symbol)
        {
            return symbol.ContainingNamespace;
        }

        public bool IsOverrideMember(ISymbol symbol)
        {
            return symbol.IsOverride;
        }

        public string GetNamespaceName(INamedTypeSymbol symbol)
        {
            return GetNamespace(symbol).ToString();
        }

        public string GetTypeName(INamedTypeSymbol symbol)
        {
            if (!symbol.IsGenericType)
                return symbol.Name;

            var typeArgNames = symbol.TypeArguments.Select(p => p.Name);
            var typeArgsStr = string.Join(", ", typeArgNames);

            return $"{symbol.Name}<{typeArgsStr}>";
        }

        public string GetConstructorName(INamedTypeSymbol symbol)
        {
            return symbol.Name;
        }

        public IEnumerable<INamedTypeSymbol> GetPartialClasses(INamespaceOrTypeSymbol symbol)
        {
            if (symbol is INamedTypeSymbol type && IsClass(type) && IsPartial(type))
                yield return type;

            foreach (var member in symbol.GetMembers())
            {
                if (member is INamespaceOrTypeSymbol root)
                {
                    foreach (var child in GetPartialClasses(root))
                        yield return child;
                }
            }
        }
    }
}
