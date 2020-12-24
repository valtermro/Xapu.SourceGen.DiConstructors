using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xapu.SourceGen.DiConstructors.CompilationUtils;
using Xapu.SourceGen.DiConstructors.TextBuffers;

namespace Xapu.SourceGen.DiConstructors.Generators
{
    internal class TypeConstructorGenerator
    {
        private readonly ICompilationSymbolParser _symbolParser;

        public TypeConstructorGenerator(ICompilationSymbolParser symbolParser)
        {
            _symbolParser = symbolParser;
        }

        public void Execute(ISourceTextBuffer buffer, INamedTypeSymbol typeSymbol)
        {
            var selfFieldsToInject = GetSelfFieldsToInject(typeSymbol);
            var inheritedFieldsToInject = GetInheritedFieldsToInject(typeSymbol);

            if (!selfFieldsToInject.Any()) return;

            var classNamespace = _symbolParser.GetNamespaceName(typeSymbol);
            var className = _symbolParser.GetTypeName(typeSymbol);
            var constructorName = _symbolParser.GetConstructorName(typeSymbol);

            buffer.WriteLine($"namespace {classNamespace}");
            buffer.BeginBlock();

            buffer.WriteLine($"public partial class {className}");
            buffer.BeginBlock();
            WriteConstructor(buffer, constructorName, selfFieldsToInject, inheritedFieldsToInject);
            buffer.EndBlock();
            
            buffer.EndBlock();
        }

        private static void WriteConstructor(ISourceTextBuffer buffer, string name, IEnumerable<IFieldSymbol> selfFields, IEnumerable<IFieldSymbol> inheritedFields)
        {
            buffer.WriteLine($"public {name}(");
            buffer.Indent();

            WriteParameterList(buffer, inheritedFields.Concat(selfFields));

            buffer.WriteLine($")");
            buffer.Dedent();

            if (inheritedFields.Any())
                WriteBaseInitCall(buffer, inheritedFields);

            buffer.BeginBlock();
            WriteBindingList(buffer, selfFields);
            buffer.EndBlock();
        }

        private IEnumerable<IFieldSymbol> GetSelfFieldsToInject(INamedTypeSymbol typeSymbol)
        {
            foreach (var field in _symbolParser.GetFields(typeSymbol))
            {
                if (_symbolParser.HasInjectedAttribute(field))
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

            foreach (var field in GetSelfFieldsToInject(baseType))
                yield return field;
        }

        private static void WriteParameterList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            var head = fields.Take(fields.Count() - 1);
            var last = fields.Last();

            foreach (var field in head)
                buffer.WriteLine($"{field.Type} {field.Name},");

            buffer.Write($"{last.Type} {last.Name}");
        }

        private static void WriteBaseInitCall(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            buffer.Write(": base(");
            WriteBaseArgumentList(buffer, fields);
            buffer.WriteLine(")");
        }

        private static void WriteBaseArgumentList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> fields)
        {
            var head = fields.Take(fields.Count() - 1);
            var last = fields.Last();

            foreach (var field in head)
                buffer.Write($"{field.Name}, ");

            buffer.Write(last.Name);
        }

        private static void WriteBindingList(ISourceTextBuffer buffer, IEnumerable<IFieldSymbol> selfFields)
        {
            foreach (var field in selfFields)
                buffer.WriteLine($"this.{field.Name} = {field.Name};");
        }
    }
}