using System;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xapu.SourceGen.DiConstructors.CompilationUtils;
using Xapu.SourceGen.DiConstructors.Generators;
using Xapu.SourceGen.DiConstructors.TextBuffers;

namespace Xapu.SourceGen.DiConstructors
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        private const string AdditionalFileXmlRootNodeName = "DiConstructorGeneratorConfig";

        private GeneratorExecutionContext ExecutionContext;
        private DefaultCompilationSymbolParser SymbolParser;
        private InjectedAttributeGenerator AttributeGenerator;
        private TypeConstructorGenerator ConstructorGenerator;

        public string InjectedAttributeName { get; private set; }
        public string InjectedAttributeNamespace { get; private set; }

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            ExecutionContext = context;

            var config = LoadGenerationConfig();

            SymbolParser = new DefaultCompilationSymbolParser();
            AttributeGenerator = new InjectedAttributeGenerator(config);
            ConstructorGenerator = new TypeConstructorGenerator(config, SymbolParser);

            GenerateInjectedAttribute();
            GenerateInjectorConstructors();
        }

        private GenerationConfig LoadGenerationConfig()
        {
            var configXDocument = GetAdditionalConfig();

            return GenerationConfig.FromXDocument(configXDocument);
        }

        private XDocument GetAdditionalConfig()
        {
            foreach (var additionalFile in ExecutionContext.AdditionalFiles)
            {
                if (!additionalFile.Path.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                    continue;

                var text = additionalFile.GetText().ToString();
                var config = XDocument.Parse(text);

                if (config.Root.Name == AdditionalFileXmlRootNodeName)
                    return config;
            }

            return default;
        }

        private void GenerateInjectedAttribute()
        {
            var buffer = new DefaultSourceTextBuffer();

            AttributeGenerator.Execute(buffer);

            AddSourceIfNotEmpty($"{InjectedAttributeName}Attribute", buffer);
        }

        private void GenerateInjectorConstructors()
        {
            var compilation = (CSharpCompilation)ExecutionContext.Compilation;

            foreach (var typeSymbol in SymbolParser.GetPartialClasses(compilation.GlobalNamespace))
            {
                var buffer = new DefaultSourceTextBuffer();

                ConstructorGenerator.Execute(buffer, typeSymbol);

                AddSourceIfNotEmpty(typeSymbol.Name, buffer);
            }
        }

        private void AddSourceIfNotEmpty(string className, ISourceTextBuffer sourceBuffer)
        {
            var hintName = $"{className}.g.cs";
            var sourceText = sourceBuffer.ToString();

            if (!string.IsNullOrWhiteSpace(sourceText))
                ExecutionContext.AddSource(hintName, sourceText);
        }
    }
}
