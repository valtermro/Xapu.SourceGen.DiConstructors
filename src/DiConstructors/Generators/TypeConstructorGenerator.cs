using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xapu.SourceGen.DiConstructors.CompilationUtils;
using Xapu.SourceGen.DiConstructors.TextBuffers;

namespace Xapu.SourceGen.DiConstructors.Generators
{
    internal class TypeConstructorGenerator
    {
        private readonly GenerationConfig _config;
        private readonly ICompilationSymbolParser _symbolParser;

        public TypeConstructorGenerator(GenerationConfig config, ICompilationSymbolParser symbolParser)
        {
            _config = config;
            _symbolParser = symbolParser;
        }

        public void Execute(ISourceTextBuffer buffer, INamedTypeSymbol typeSymbol)
        {
            var ownFieldsToInject = GetOwnFieldsToInject(typeSymbol);
            var inheritedFieldsToInject = GetInheritedFieldsToInject(typeSymbol);

            if (!ownFieldsToInject.Any() && !inheritedFieldsToInject.Any())
                return;

            var classNamespace = _symbolParser.GetNamespaceName(typeSymbol);
            var className = _symbolParser.GetTypeName(typeSymbol);

            buffer.WriteLine($"namespace {classNamespace}");
            buffer.BeginBlock();

            buffer.WriteLine($"public partial class {className}");
            buffer.BeginBlock();
            WriteConstructor(buffer, typeSymbol, ownFieldsToInject, inheritedFieldsToInject);
            buffer.EndBlock();

            buffer.EndBlock();
        }

        private void WriteConstructor(ISourceTextBuffer buffer, INamedTypeSymbol typeSymbol, IEnumerable<IFieldSymbol> ownFields, IEnumerable<IFieldSymbol> inheritedFields)
        {
            var name = _symbolParser.GetConstructorName(typeSymbol);

            buffer.WriteLine($"public {name}(");
            buffer.Indent();

            WriteParameterList(buffer, inheritedFields.Concat(ownFields));

            buffer.WriteLine($")");
            buffer.Dedent();

            if (inheritedFields.Any())
                WriteBaseInitCall(buffer, inheritedFields);

            buffer.BeginBlock();
            WriteBindingList(buffer, ownFields);
            ResolvePostControllerMethodCall(buffer, typeSymbol);
            buffer.EndBlock();
        }

        private void ResolvePostControllerMethodCall(ISourceTextBuffer buffer, INamedTypeSymbol typeSymbol)
        {
            var postControllerMethod = GetPostControllerMethod(typeSymbol);
            
            if (postControllerMethod == null)
                return;

            if (_symbolParser.IsOverrideMember(postControllerMethod))
                throw new Exception($"The original '{_config.PostConstructorMethodName}' method should run after the constructor of the class where it's defined. Do not override it.");

            buffer.WriteLine($"{_config.PostConstructorMethodName}();");
        }

        private IMethodSymbol GetPostControllerMethod(INamedTypeSymbol symbol)
        {
            foreach (var method in _symbolParser.GetOwnMethods(symbol))
            {
                if (method.Name == _config.PostConstructorMethodName)
                    return method;
            }
            return default;
        }

        private IEnumerable<IFieldSymbol> GetOwnFieldsToInject(INamedTypeSymbol typeSymbol)
        {
            foreach (var field in _symbolParser.GetOwnFields(typeSymbol))
            {
                if (HasAutoInjectAttribute(field))
                    yield return field;
            }
        }

        private IEnumerable<IFieldSymbol> GetInheritedFieldsToInject(INamedTypeSymbol typeSymbol)
        {
            var baseType = _symbolParser.GetBaseType(typeSymbol);

            if (baseType == null)
                yield break;

            foreach (var field in GetInheritedFieldsToInject(baseType))
                yield return field;

            foreach (var field in GetOwnFieldsToInject(baseType))
                yield return field;
        }

        private bool HasAutoInjectAttribute(ISymbol symbol)
        {
            foreach (var attribute in _symbolParser.GetAttributes(symbol))
            {
                if (attribute.AttributeClass.ToString() == typeof(InjectedAttribute).FullName)
                    return true;
            }
            return false;
        }

        private void WriteParameterList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            var head = fields.Take(fields.Count() - 1);
            var last = fields.Last();

            foreach (var field in head)
                buffer.WriteLine($"{field.Type} {field.Name},");

            buffer.Write($"{last.Type} {last.Name}");
        }

        private void WriteBaseInitCall(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            buffer.Write(": base(");
            WriteBaseArgumentList(buffer, fields);
            buffer.WriteLine(")");
        }

        private void WriteBaseArgumentList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            var head = fields.Take(fields.Count() - 1);
            var last = fields.Last();

            foreach (var field in head)
                buffer.Write($"{field.Name}, ");

            buffer.Write(last.Name);
        }

        private void WriteBindingList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            foreach (var field in fields)
                buffer.WriteLine($"this.{field.Name} = {field.Name};");
        }
    }
}