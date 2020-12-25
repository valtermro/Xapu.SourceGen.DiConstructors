using System;
using System.Runtime.CompilerServices;
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
        private TypeConstructorGenerator ConstructorGenerator;

        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                ExecutionContext = context;
                DoExecute();
            }
            catch (Exception e)
            {
                var descriptor = new DiagnosticDescriptor(
                    id: "DCG0001",
                    title: $"An exception was thrown by {typeof(Generator).FullName}",
                    messageFormat: $"An exception was thrown by {typeof(Generator).FullName}: '{{0}}'",
                    category: typeof(Generator).FullName,
                    defaultSeverity: DiagnosticSeverity.Error,
                    isEnabledByDefault: true);

                context.ReportDiagnostic(Diagnostic.Create(descriptor, Location.None, e.Message));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void DoExecute()
        {
            var config = LoadGenerationConfig();

            SymbolParser = new DefaultCompilationSymbolParser();
            ConstructorGenerator = new TypeConstructorGenerator(config, SymbolParser);

            GenerateConstructors();
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

        private void GenerateConstructors()
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
